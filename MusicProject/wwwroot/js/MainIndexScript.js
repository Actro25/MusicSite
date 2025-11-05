const list = document.getElementById('suggestions');

window.addEventListener('MainTrackReceived', (e) => {
    displayPlatformResults(e.tracks, e.platformTracks)
})
function displayPlatformResults(tracks,platform) {
    tracks.forEach(track => {
        const li = document.createElement('li');
        const img = document.createElement('img');
        let text = new Text();
        if (platform === 'Spotify') {
            img.src = '/img/spotify-logo-png.png';
            text = document.createTextNode(`${track.name} — ${track.artists.join(", ")}`);
        }
        else if (platform === 'SoundCloud') {
            img.src = '/img/soundcloud-logo-png.png';
            text = document.createTextNode(`${track.name} — ${track.artist}`);
        }
        img.height = 20;
        img.width = 20;
        img.style.verticalAlign = 'middle';
        img.style.marginRight = '8px';
        img.alt = `${platform} logo`;

        li.appendChild(text);
        li.appendChild(img);

        list.appendChild(li);
    });

    list.classList.toggle('hidden', tracks.length === 0);
}
function displayRegularResults(filtered, icon) {
    filtered.forEach(opt => {
        const li = document.createElement('li');

        if (icon) {
            const img = document.createElement('img');
            img.src = icon;
            img.height = 20;
            img.width = 20;
            img.style.verticalAlign = 'middle';
            img.style.marginRight = '8px';
            li.appendChild(img);
        }

        const text = document.createTextNode(opt);
        li.appendChild(text);

        li.onclick = () => {
            input.value = opt;
            list.classList.add('hidden');
        };

        list.appendChild(li);
    });

    list.classList.toggle('hidden', filtered.length === 0);
}