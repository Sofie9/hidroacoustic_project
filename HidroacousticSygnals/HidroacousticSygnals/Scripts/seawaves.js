$(document).ready(function () {

    // make some waves.
    var ocean = document.getElementById("ocean"),
        waveWidth = 10,
        waveCount = Math.floor(window.innerWidth-10 / waveWidth),
        docFrag = document.createDocumentFragment();
    for (var i = 0; i < waveCount; i++) {
        var wave = document.createElement("div");
        wave.className += " wave";
        docFrag.appendChild(wave);
        wave.style.left = i * waveWidth + "px";
        wave.style.webkitAnimationDelay = (i / 100) + "s";

        var wave_middle = document.createElement("div");
        wave_middle.className += " wave_middle";
        docFrag.appendChild(wave_middle);
        wave_middle.style.left = i * waveWidth + "px";
        wave_middle.style.webkitAnimationDelay = (i / 91) + "s";

        var wave_bottom = document.createElement("div");
        wave_bottom.className += " wave_bottom";
        docFrag.appendChild(wave_bottom);
        wave_bottom.style.left = i * waveWidth + "px";
        wave_bottom.style.webkitAnimationDelay = (i / 97) + "s";

        var wave_light = document.createElement("div");
        wave_light.className += " wave_light";
        docFrag.appendChild(wave_light);
        wave_light.style.left = i * waveWidth + "px";
        wave_light.style.webkitAnimationDelay = 0 + "s";

    }
    ocean.appendChild(docFrag);
});