(() => {
    'use strict';

    const ROOT_ID = 'jellyfinBulletin';
    let lastSignature = '';
    let lastData = null;
    let refreshTimer = null;

    function findHome() {
        return document.querySelector(
            '#indexPage:not(.hide) .homeSectionsContainer, ' +
            '#indexPage:not(.hide) .sections, ' +
            '#indexPage:not(.hide) .content-primary, ' +
            '.homePage:not(.hide) .homeSectionsContainer, ' +
            '.homePage:not(.hide) .sections, ' +
            '.homePage:not(.hide) .content-primary'
        );
    }

    function validImageUrl(url) {
        return /^https?:\/\//i.test(url || '')
            || /^\/Bulletin\/Image\/[0-9a-f]{32}\.(png|jpg|webp)$/i.test(url || '');
    }

    function createInline(run) {
        let element = document.createElement('span');
        element.textContent = run.Text || run.text || '';

        if (run.Bold || run.bold) element.style.fontWeight = '700';
        if (run.Italic || run.italic) element.style.fontStyle = 'italic';
        if (run.Underline || run.underline) element.style.textDecoration = 'underline';

        const color = run.Color || run.color;
        if (/^#[0-9a-f]{6}$/i.test(color || '')) element.style.color = color;

        const href = run.Href || run.href;
        if (href && /^https?:\/\//i.test(href)) {
            const link = document.createElement('a');
            link.href = href;
            link.target = '_blank';
            link.rel = 'noopener noreferrer';
            link.append(element);
            element = link;
        }

        return element;
    }

    function appendInlines(parent, runs) {
        (runs || []).forEach(run => parent.append(createInline(run)));
    }

    function createContent(item) {
        const content = document.createElement('div');
        content.className = 'bulletin-content';

        (item.Blocks || item.blocks || []).forEach(block => {
            const type = block.Type || block.type;
            if (type === 'paragraph') {
                const paragraph = document.createElement('p');
                appendInlines(paragraph, block.Content || block.content);
                content.append(paragraph);
                return;
            }

            if (type === 'bulletList' || type === 'numberedList') {
                const list = document.createElement(type === 'bulletList' ? 'ul' : 'ol');
                (block.Items || block.items || []).forEach(runs => {
                    const listItem = document.createElement('li');
                    appendInlines(listItem, runs);
                    list.append(listItem);
                });
                content.append(list);
            }
        });

        return content;
    }

    function render(data) {
        const items = data.Items || data.items || [];
        if (!items.length) {
            document.getElementById(ROOT_ID)?.remove();
            return;
        }

        const home = findHome();
        if (!home) return;

        const signature = JSON.stringify(items);
        const existing = document.getElementById(ROOT_ID);
        if (signature === lastSignature && existing) {
            if (existing.parentElement !== home) home.prepend(existing);
            return;
        }
        lastSignature = signature;

        const root = document.createElement('section');
        root.id = ROOT_ID;
        root.className = 'jellyfin-bulletin';
        root.setAttribute('aria-label', 'News');

        const feature = document.createElement('div');
        feature.className = 'bulletin-feature';

        const body = document.createElement('article');
        body.className = 'bulletin-active';
        const title = document.createElement('h2');
        const date = document.createElement('time');
        const contentHost = document.createElement('div');
        const image = document.createElement('img');
        image.className = 'bulletin-image';
        image.loading = 'lazy';
        image.hidden = true;

        body.append(title, date, contentHost);
        feature.append(body, image);
        root.append(feature);

        const tabs = document.createElement('div');
        tabs.className = 'bulletin-tabs';
        tabs.setAttribute('role', 'tablist');
        root.append(tabs);

        function select(index) {
            const item = items[index];
            title.textContent = item.Title || item.title;
            const published = item.PublishedAt || item.publishedAt;
            date.dateTime = published;
            date.textContent = new Intl.DateTimeFormat(undefined, {
                year: 'numeric',
                month: 'long',
                day: 'numeric'
            }).format(new Date(published));
            contentHost.replaceChildren(createContent(item));
            const imageUrl = item.ImageUrl || item.imageUrl;
            if (validImageUrl(imageUrl)) {
                image.src = imageUrl;
                image.alt = item.Title || item.title || '';
                image.hidden = false;
                feature.classList.add('has-image');
            } else {
                image.removeAttribute('src');
                image.alt = '';
                image.hidden = true;
                feature.classList.remove('has-image');
            }
            [...tabs.children].forEach((button, buttonIndex) => {
                button.setAttribute('aria-selected', String(buttonIndex === index));
            });
        }

        items.forEach((item, index) => {
            const button = document.createElement('button');
            button.type = 'button';
            button.className = 'bulletin-tab';
            button.setAttribute('role', 'tab');
            button.textContent = item.Title || item.title;
            button.addEventListener('click', () => select(index));
            tabs.append(button);
        });

        select(0);
        document.getElementById(ROOT_ID)?.remove();
        home.prepend(root);
    }

    async function refresh() {
        if (!findHome()) return;

        try {
            const data = await ApiClient.ajax({
                type: 'GET',
                url: ApiClient.getUrl('Bulletin/News', { refresh: Date.now() }),
                dataType: 'json'
            });
            lastData = data;
            render(data);
        } catch (error) {
            console.debug('Bulletin could not load', error);
        }
    }

    function maintainWidget() {
        if (!findHome()) return;
        if (lastData) render(lastData);
        if (!lastData || !document.getElementById(ROOT_ID)) {
            clearTimeout(refreshTimer);
            refreshTimer = setTimeout(refresh, 100);
        }
    }

    document.addEventListener('viewshow', refresh);
    window.addEventListener('hashchange', refresh);
    window.addEventListener('jellyfin-bulletin-refresh', refresh);
    new MutationObserver(maintainWidget).observe(document.body, {
        childList: true,
        subtree: true
    });
    setInterval(refresh, 15000);
    refresh();
})();
