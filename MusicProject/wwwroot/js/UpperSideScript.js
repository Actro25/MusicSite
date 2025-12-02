const input = document.getElementById('search');
const homeButton = document.getElementById('home-button');
const suggestionWindow = document.getElementById('main-site-div');
const playlistWindow = document.getElementById('playlists-music-div');
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
homeButton.addEventListener('click', () => {
    window.dispatchEvent(new CustomEvent('HideSugWinAndPlayWin'));
});
input.addEventListener('input', async () => {
    window.dispatchEvent(new CustomEvent('ChangeShowDiv'));
    const value = input.value.toLowerCase();
    list.innerHTML = '';

    if (!value) {
        list.classList.add('hidden');
        playlistsUl.classList.add('hidden');
        return;
    }

    let searchValue = value;

    if (value.startsWith('spotify:')) {
        searchValue = value.replace('spotify:', '').trim();

        clearTimeout(timeoutId);

        timeoutId = setTimeout(() => {
            if (searchValue.trim()) {
                connection.invoke("SendSpotifyMusic", searchValue);
                connection.invoke("GetPlaylists", searchValue, "Spotify");
            }
        }, 300);
    }
    else if (value.startsWith('soundcloud:')) {
        searchValue = value.replace('soundcloud:', '').trim();

        clearTimeout(timeoutId);
        
        timeoutId = setTimeout(() => {
            if (searchValue.trim()) {
                connection.invoke("SendSoundCloudMusic", searchValue);
                connection.invoke("GetPlaylists", searchValue, "SoundCloud");
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
        window.dispatchEvent(new CustomEvent('MainTrackReceived', {
            detail: {
                tracks: message,
                platformTracks: 'Spotify'
            }
        }));
    }
});
connection.on("ReceiveSoundCloudMusic", (message) => {
    if (message) {
        window.dispatchEvent(new CustomEvent('MainTrackReceived', {
            detail: {
                tracks: message,
                platformTracks: 'SoundCloud'
            }
        }));
    }
});
connection.on("ReceivePlayList", (message) => {
    if (message) {
        window.dispatchEvent(new CustomEvent('MainPlayListsReceived', {
            detail: message
        }));
    }
});
connection.on("ReceiveOneTrack", (message) => {
    if (message) {
        const audio = message.audios;

        const track = message.track;

        const img = track.img;
        const trackId = track.trackId;
        const trackName = track.trackName;
        const artists = track.artistsNames;
        const trackUrl = track.trackUrl;
        window.dispatchEvent(new CustomEvent('RightSidePanelReceive', {detail: 
                {
                    image: img,
                    trackName: trackName,
                    artists: artists,
                    trackUrl: trackUrl
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