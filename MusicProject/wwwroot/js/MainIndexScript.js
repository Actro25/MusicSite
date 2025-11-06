const list = document.getElementById('suggestions');

window.addEventListener('MainTrackReceived', (e) => {
    displayPlatformResults(e.tracks, e.platformTracks)
})
function displayPlatformResults(tracks, platform) {
    if (!tracks || typeof tracks !== 'object') {
        list.classList.add('hidden');
        return;
    }

    list.innerHTML = '';

    Object.values(tracks).forEach(track => {
        if (!track || typeof track !== 'object') {
            return;
        }

        const li = document.createElement('li');
        const img = document.createElement('img');
        let text;

        if (platform === 'Spotify') {
            img.src = '/img/spotify-logo-png.png';
            const artists = track.artists && Array.isArray(track.artists)
                ? track.artists.join(", ")
                : 'Unknown Artist';
            text = document.createTextNode(`${track.name || 'Unknown Track'} - ${artists}`);
        }
        else if (platform === 'SoundCloud') {
            img.src = '/img/soundcloud-logo-png.png';
            const artist = track.artist || 'Unknown Artist';
            text = document.createTextNode(`${track.name || 'Unknown Track'} - ${artist}`);
        } 
        else {
            text = document.createTextNode(track.name || 'Unknown Track');
        }

        img.height = 20;
        img.width = 20;
        img.style.verticalAlign = 'middle';
        img.style.marginRight = '8px';
        img.alt = `${platform} logo`;

        li.appendChild(text);
        li.appendChild(img);
        li.onclick = () => {
            window.dispatchEvent(new CustomEvent('InfoAboutTrackSend', {
                detail: {
                    idTrack: track.id,
                    platformTrack: platform
                }
            }));
        };
        list.appendChild(li);
    });

    const isEmpty = Object.keys(tracks).length === 0;
    list.classList.toggle('hidden', isEmpty);
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