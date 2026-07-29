(() => {
    'use strict';

    const ROOT_ID = 'jellyfinBulletin';
    let lastSignature = '';
    let lastData = null;
    let refreshTimer = null;
    let rotationTimer = null;

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

        const autoRotate = Boolean(data.AutoRotate ?? data.autoRotate ?? true);
        const requestedPanelHeight = data.PanelHeight ?? data.panelHeight;
        const panelHeight = requestedPanelHeight === 'compact' || requestedPanelHeight === 'tall'
            ? requestedPanelHeight
            : 'standard';
        const showImages = Boolean(data.ShowImages ?? data.showImages ?? true);
        const rotationSeconds = Math.max(5, Math.min(30,
            Number(data.RotationIntervalSeconds ?? data.rotationIntervalSeconds ?? 9)));
        const signature = JSON.stringify({ items, autoRotate, rotationSeconds, panelHeight, showImages });
        const existing = document.getElementById(ROOT_ID);
        if (signature === lastSignature && existing) {
            if (existing.parentElement !== home) home.prepend(existing);
            return;
        }
        lastSignature = signature;

        const root = document.createElement('section');
        root.id = ROOT_ID;
        root.className = `jellyfin-bulletin bulletin-height-${panelHeight}`;
        root.classList.toggle('bulletin-images-hidden', !showImages);
        root.setAttribute('aria-label', 'News');

        const feature = document.createElement('div');
        feature.className = 'bulletin-feature';

        const body = document.createElement('article');
        body.className = 'bulletin-active';
        const title = document.createElement('h2');
        const date = document.createElement('time');
        const contentHost = document.createElement('div');
        const imageFrame = document.createElement('div');
        imageFrame.className = 'bulletin-image-frame';
        const image = document.createElement('img');
        image.className = 'bulletin-image';
        image.loading = 'lazy';
        imageFrame.hidden = true;
        imageFrame.append(image);

        body.append(title, date, contentHost);
        feature.append(body, imageFrame);
        root.append(feature);

        const navigation = document.createElement('div');
        navigation.className = 'bulletin-navigation';
        const previous = document.createElement('button');
        previous.type = 'button';
        previous.className = 'bulletin-arrow';
        previous.setAttribute('aria-label', 'Previous announcement');
        previous.textContent = '‹';
        const position = document.createElement('span');
        position.className = 'bulletin-position';
        position.setAttribute('aria-live', 'polite');
        const pause = document.createElement('button');
        pause.type = 'button';
        pause.className = 'bulletin-arrow bulletin-pause';
        const next = document.createElement('button');
        next.type = 'button';
        next.className = 'bulletin-arrow';
        next.setAttribute('aria-label', 'Next announcement');
        next.textContent = '›';
        navigation.append(previous, position, pause, next);
        root.append(navigation);

        let selectedIndex = 0;
        let interactionPaused = false;
        let manuallyPaused = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

        function updatePauseButton() {
            pause.textContent = manuallyPaused ? '▶' : 'Ⅱ';
            pause.setAttribute('aria-label', manuallyPaused
                ? 'Start automatic announcement rotation'
                : 'Pause automatic announcement rotation');
            pause.setAttribute('aria-pressed', String(manuallyPaused));
            pause.hidden = !autoRotate || items.length < 2;
        }

        function formattedDate(value, options) {
            return new Intl.DateTimeFormat(undefined, options).format(new Date(value));
        }

        function restartRotation() {
            clearInterval(rotationTimer);
            if (!autoRotate || items.length < 2 || interactionPaused || manuallyPaused || document.hidden) return;
            rotationTimer = setInterval(() => {
                select((selectedIndex + 1) % items.length, selectedIndex + 1 >= items.length ? -1 : 1);
            }, rotationSeconds * 1000);
        }

        function select(index, direction = 1, restart = false) {
            selectedIndex = (index + items.length) % items.length;
            const item = items[selectedIndex];
            feature.classList.remove('slide-from-left', 'slide-from-right');
            void feature.offsetWidth;
            feature.classList.add(direction < 0 ? 'slide-from-left' : 'slide-from-right');
            title.textContent = item.Title || item.title;
            const published = item.PublishAt || item.publishAt || item.PublishedAt || item.publishedAt;
            date.dateTime = published;
            date.textContent = formattedDate(published, {
                year: 'numeric',
                month: 'long',
                day: 'numeric'
            });
            contentHost.replaceChildren(createContent(item));
            const imageUrl = item.ImageUrl || item.imageUrl;
            if (showImages && validImageUrl(imageUrl)) {
                image.src = imageUrl;
                image.alt = item.ImageAlt || item.imageAlt || '';
                imageFrame.hidden = false;
                feature.classList.add('has-image');
            } else {
                image.removeAttribute('src');
                image.alt = '';
                imageFrame.hidden = true;
                feature.classList.remove('has-image');
            }
            position.textContent = `${selectedIndex + 1} of ${items.length}`;
            if (restart) restartRotation();
        }

        previous.addEventListener('click', () => select(selectedIndex - 1, -1, true));
        next.addEventListener('click', () => select(selectedIndex + 1, 1, true));
        pause.addEventListener('click', () => {
            manuallyPaused = !manuallyPaused;
            updatePauseButton();
            restartRotation();
        });
        root.addEventListener('mouseenter', () => {
            interactionPaused = true;
            clearInterval(rotationTimer);
        });
        root.addEventListener('mouseleave', () => {
            interactionPaused = false;
            restartRotation();
        });
        root.addEventListener('focusin', () => {
            interactionPaused = true;
            clearInterval(rotationTimer);
        });
        root.addEventListener('focusout', event => {
            if (root.contains(event.relatedTarget)) return;
            interactionPaused = false;
            restartRotation();
        });

        updatePauseButton();
        select(0, 1);
        document.getElementById(ROOT_ID)?.remove();
        home.prepend(root);
        restartRotation();
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
