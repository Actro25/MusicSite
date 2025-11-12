const input = document.getElementById('search');

const spotifySearch = ['SpotifyMusic1','SpotifyMusic2','SpotifyMusic3','SpotifyMusic4'];
const soundCloudSearch = ['SoundCloudMusic1','SoundCloudMusic2','SoundCloudMusic3'];
const options = [...spotifySearch,...soundCloudSearch];
let timeoutId;
"use strict";
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/musicHub")
    .build();

connection.start()
    .then(() => {})
    .catch(err => console.error("❌ SignalR connection error:", err));
window.addEventListener('InfoAboutTrackSend', (e) => {
    connection.invoke("GetOneTrack", e.detail.idTrack, e.detail.platformTrack, e.detail.nameTrack, e.detail.artistTrack);
});
input.addEventListener('input', async () => {
    const value = input.value.toLowerCase();
    list.innerHTML = '';

    if (!value) {
        list.classList.add('hidden');
        return;
    }

    let searchValue = value;

    if (value.startsWith('spotify:')) {
        searchValue = value.replace('spotify:', '').trim();

        clearTimeout(timeoutId);

        timeoutId = setTimeout(() => {
            if (searchValue.trim()) {
                connection.invoke("SendSpotifyMusic", searchValue);
            }
        }, 300);
    }
    else if (value.startsWith('soundcloud:')) {
        searchValue = value.replace('soundcloud:', '').trim();

        clearTimeout(timeoutId);
        
        timeoutId = setTimeout(() => {
            if (searchValue.trim()) {
                connection.invoke("SendSoundCloudMusic", searchValue)
            }
        }, 300)
    }
    else {
        const filtered = options.filter(opt =>
            opt.toLowerCase().includes(value)
        );
        displayRegularResults(filtered, '');
    }
});
connection.on("ReceiveSpotifyMusic", (message) => {
 
    if (message) {
        displayPlatformResults(message, 'Spotify');
        window.dispatchEvent(new CustomEvent('MainTrackReceived', {tracks : message, platformTracks: 'Spotify'}));
    }
});
connection.on("ReceiveSoundCloudMusic", (message) => {
    if (message) {
        displayPlatformResults(message, 'SoundCloud');
        window.dispatchEvent(new CustomEvent('MainTrackReceived', {tracks : message, platformTracks: 'SoundCloud'}));
    }
})
connection.on("ReceiveOneTrack", (message) => {
    if (message) {
        const audio = message.audios;

        const track = message.track;

        const img = track.img;
        const trackId = track.trackId;
        const trackName = track.trackName;
        const artists = track.artistsNames;
        window.dispatchEvent(new CustomEvent('RightSidePanelReceive', {detail: 
                {
                    image: img,
                    trackName: trackName,
                    artists: artists
                }}));
        window.dispatchEvent(new CustomEvent('BottomSidePanelReceive', {detail: 
                {
                    image: img,
                    trackName: trackName,
                    artists: artists,
                    audios: audio
                }}))
    }
})