const input = document.getElementById('search');
const list = document.getElementById('suggestions');

const spotifySearch = ['SpotifyMusic1','SpotifyMusic2','SpotifyMusic3','SpotifyMusic4'];
const soundCloudSearch = ['SoundCloudMusic1','SoundCloudMusic2','SoundCloudMusic3'];
const options = [...spotifySearch,...soundCloudSearch];
let timeoutId;
"use strict";
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/musicHub")
    .build();

connection.start()
    .then(() => {
        console.log("✅ SignalR connected!");
    })
    .catch(err => console.error("❌ SignalR connection error:", err));


input.addEventListener('input', async () => {
    const value = input.value.toLowerCase();
    list.innerHTML = '';

    if (!value) {
        list.classList.add('hidden');
        return;
    }

    let refImg = '';
    let searchValue = value;
    let hasPrefix = false;

    if (value.startsWith('spotify:')) {
        refImg = '/img/spotify-logo-png.png';
        searchValue = value.replace('spotify:', '').trim();
        hasPrefix = true;

        clearTimeout(timeoutId);

        timeoutId = setTimeout(() => {
            if (searchValue.trim()) {
                connection.invoke("SendMusic", searchValue);
            }
        }, 300);
    }
    else if (value.startsWith('soundcloud:')) {
        refImg = '/img/soundcloud-logo-png.png';
        searchValue = value.replace('soundcloud:', '').trim();
        hasPrefix = true;

        const filtered = soundCloudSearch.filter(opt =>
            opt.toLowerCase().includes(searchValue)
        );
        displayRegularResults(filtered, refImg);
    }
    else {
        const filtered = options.filter(opt =>
            opt.toLowerCase().includes(value)
        );
        displayRegularResults(filtered, '');
    }
});
connection.on("ReceiveMusic", (message) => {
 
    if (message && Array.isArray(message)) {
        displaySpotifyResults(message);
    }
});

/*function updateSuggestions(tracks) {

    list.innerHTML = '';

    if (tracks.length === 0) {
        list.classList.add('hidden');
        return;
    }

    tracks.forEach(track => {
        const li = document.createElement('li');
        li.textContent = track;
        li.style.padding = '8px';
        li.style.cursor = 'pointer';
        li.style.borderBottom = '1px solid #eee';

        li.onclick = () => {
            input.value = track;
            list.classList.add('hidden');
        };

        list.appendChild(li);
    });

    console.log('👁️ Показуємо список');
    list.classList.remove('hidden');
}*/
function displaySpotifyResults(tracks) {
    tracks.forEach(track => {
        const li = document.createElement('li');

/*        if (icon) {
            const img = document.createElement('img');
            img.src = icon;
            img.height = 20;
            img.width = 20;
            img.style.verticalAlign = 'middle';
            img.style.marginRight = '8px';
            li.appendChild(img);
        }*/

        
        const text = document.createTextNode(track);
        li.appendChild(text);

        li.onclick = () => {
            input.value = `spotify:${track.name}`;
            list.classList.add('hidden');
        };

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