const list = document.getElementById('suggestions');
const playlistsUl = document.getElementById('suggestions-playlists');
const playListMusicInnerUl = document.getElementById('suggestions-playlists-music');
const mainDiv = document.getElementById('main-site-div');
const mainSiteWindow = document.getElementById('main-window-site-div');
const playListMusicDiv = document.getElementById('playlists-music-div');
const playListMusicUl = document.getElementById('suggestions-playlists-music');
const playListMusicButton = document.getElementById('play-button-playlist');
let playlistTracks;
let platformPlayList;
window.addEventListener('MainPlayListsReceived', (e) => {
    displayPlatformResultPlayList(e.detail.playlists, e.detail.platform)
});
window.addEventListener('MainTrackReceived', (e) => {
    displayPlatformTrackResults(e.detail.tracks, e.detail.platformTracks);
});
window.addEventListener('ChangeShowDiv', (e) => {
    playListMusicDiv.classList.add('hidden');
    mainDiv.classList.remove('hidden');
    mainSiteWindow.classList.add('hidden');
});
window.addEventListener('HideSugWinAndPlayWin', (e) => {
    mainDiv.classList.add('hidden');
    playListMusicDiv.classList.add('hidden');
    mainSiteWindow.classList.remove('hidden');
});
playListMusicButton.addEventListener('click', () => {
    if (playlistTracks !== null) {
        let artist = "";
        if (platformPlayList === 'Spotify') {
            artist = playlistTracks[0].artistsNames
                .map(a => a.nameArtist)
                .join(", ");
        }
        else if (platformPlayList === 'SoundCloud') {
            artist = playlistTracks[0].artistsNames
                .map(a => a.nameArtist)
                .join(", ");
        }
        isPlayListMusic = true;
        window.dispatchEvent(new CustomEvent('BottomPlayListMusics', {
            detail: {
                playlistTracks = playlistTracks,
                platformPlayList = platformPlayList,
                isPlayListMusic = true,
                currentIdMusic = 0
            }
        }));
        window.dispatchEvent(new CustomEvent('InfoAboutTrackSend', {
            detail: {
                idTrack: playlistTracks[0].trackId,
                platformTrack: platformPlayList,
                nameTrack: playlistTracks[0].trackName,
                artistTrack: artist
            }
        }));
    }
});
function displayPlatformResultPlayList(playlists, platform) {
    for (const playlist of Object.values(playlists)) {
        if (!playlist.name && !playlist.id) {
            continue;
        }
        const li = document.createElement('li');
        const previewPlaylistImage = document.createElement('img')
        let textPlayList = document.createElement('h3');
        let artist = document.createElement('p');

        previewPlaylistImage.src = playlist.urlImage;
        previewPlaylistImage.alt = `Track icon`;

        previewPlaylistImage.classList.add('cardImgPlayList');

        li.classList.add('cardPlayList');

        textPlayList.innerText = playlist.name;
        artist.innerText = playlist.artist.map(item => item.nameArtist).join(", ");

        li.appendChild(previewPlaylistImage);
        li.appendChild(textPlayList);
        li.appendChild(artist);
        li.onclick = () => {
            mainDiv.classList.add('hidden');
            playListMusicDiv.classList.remove('hidden');
            displayPlayListMusicInside(playlist, platform);
        };
        playlistsUl.appendChild(li);
    }
    const isEmpty = Object.keys(playlists).length === 0;
    playlistsUl.classList.toggle('hidden', isEmpty);
}
function displayPlayListMusicInside(playlist, platform) {
    platformPlayList = platform;
    playListMusicInnerUl.innerHTML = '';
    let quantity = 1;

    const playListImage = document.getElementById('playlist-image');
    playListImage.src = playlist.urlImage;
    playListImage.width = 200;
    playListImage.height = 200;
    playListImage.alt = `Track icon`;
    playListImage.style.verticalAlign = 'middle';
    playListImage.style.borderRadius = "5px"

    const playListName = document.getElementById('playlist-name');
    playListName.innerText = playlist.name;

    const playListArtist = document.getElementById('playlist-artist');
    playListArtist.innerText = playlist.artist
        .map(a => a.nameArtist)
        .join(", ");
    playlistTracks = playlist.tracks;
    Object.values(playlist.tracks).forEach(track => {
        if (!track || typeof track !== 'object') {
            return;
        }
        const playListMusicId = document.createElement('p');
        const li = document.createElement('li');
        const previewTrackImage = document.createElement('img');
        const img = document.createElement('img');
        let text;
        let artist = "";

        playListMusicId.innerText = `${quantity}.`;

        previewTrackImage.src = track.img;
        previewTrackImage.width = 50;
        previewTrackImage.height = 50;
        previewTrackImage.alt = `Track icon`;
        previewTrackImage.style.borderRadius = "15px"
        previewTrackImage.style.verticalAlign = 'middle';
        previewTrackImage.style.marginRight = '10px';

        if (platform === 'Spotify') {
            artist = track.artistsNames
                .map(a => a.nameArtist)
                .join(", ");
            text = document.createTextNode(`${track.trackName || 'Unknown Track'} - ${artist}`);
        }
        else if (platform === 'SoundCloud') {
            artist = track.artistsNames
                .map(a => a.nameArtist)
                .join(", ");
            text = document.createTextNode(`${track.trackName || 'Unknown Track'} - ${artist}`);
        }
        else {
            text = document.createTextNode(track.name || 'Unknown Track');
        }
        li.appendChild(playListMusicId);
        li.appendChild(previewTrackImage);
        li.appendChild(text);
        li.onclick = () => {
            window.dispatchEvent(new CustomEvent('BottomPlayListMusics', {
                detail: {
                    playlistTracks = playlistTracks,
                    platformPlayList = platformPlayList,
                    isPlayListMusic = true,
                    currentIdMusic = (quantity - 1)
                }
            }));
            window.dispatchEvent(new CustomEvent('InfoAboutTrackSend', {
                detail: {
                    idTrack: track.trackId,
                    platformTrack: platform,
                    nameTrack: track.trackName,
                    artistTrack: artist
                }
            }));
        };
        playListMusicUl.appendChild(li);
        quantity = quantity + 1;
    });
}
function displayPlatformTrackResults(tracks, platform) {
    if (!tracks || typeof tracks !== 'object') {
        list.classList.add('hidden');
        return;
    }
    list.innerHTML = '';
    playlistsUl.innerHTML = '';
    Object.values(tracks).forEach(track => {
        if (!track || typeof track !== 'object') {
            return;
        }

        const li = document.createElement('li');
        const previewTrackImage = document.createElement('img');
        const img = document.createElement('img');
        let text;
        let artist = "";

        previewTrackImage.src = track.image;
        previewTrackImage.width = 50;
        previewTrackImage.height = 50;
        previewTrackImage.alt = `Track icon`;
        previewTrackImage.style.borderRadius = "15px"
        previewTrackImage.style.verticalAlign = 'middle';
        previewTrackImage.style.marginRight = '10px';
        if (platform === 'Spotify') {
            img.src = '/img/spotify-logo-png.png';
            artist = track.artists && Array.isArray(track.artists)
                ? track.artists.join(", ")
                : 'Unknown Artist';
            text = document.createTextNode(`${track.name || 'Unknown Track'} - ${artist}`);
        }
        else if (platform === 'SoundCloud') {
            img.src = '/img/soundcloud-logo-png.png';
            artist = track.artist || 'Unknown Artist';
            text = document.createTextNode(`${track.name || 'Unknown Track'} - ${artist}`);
        } 
        else {
            text = document.createTextNode(track.name || 'Unknown Track');
        }


        img.height = 20;
        img.width = 20;
        img.style.verticalAlign = 'middle';
        img.alt = `${platform} logo`;

        li.appendChild(previewTrackImage);
        li.appendChild(text);
        li.appendChild(img);
        li.onclick = () => {
            window.dispatchEvent(new CustomEvent('InfoAboutTrackSend', {
                detail: {
                    idTrack: track.id,
                    platformTrack: platform,
                    nameTrack: track.name,
                    artistTrack: artist
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