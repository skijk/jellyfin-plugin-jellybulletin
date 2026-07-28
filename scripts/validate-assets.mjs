import { readFile } from 'node:fs/promises';

const clientScript = await readFile(
    new URL('../Jellyfin.Plugin.JellyBulletin/Web/bulletin.js', import.meta.url),
    'utf8'
);
new Function(clientScript);

const configPage = await readFile(
    new URL('../Jellyfin.Plugin.JellyBulletin/Configuration/configPage.html', import.meta.url),
    'utf8'
);
const embeddedScript = configPage.match(/<script[^>]*>([\s\S]*?)<\/script>/)?.[1];
if (!embeddedScript) {
    throw new Error('The configuration page does not contain an embedded script.');
}

new Function(embeddedScript);
console.log('JellyBulletin JavaScript assets are valid.');
