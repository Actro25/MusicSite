const el = document.getElementById("resizable-left");
const handle = document.getElementById("resize-line-left");

let resizing_left = false;
let startX, startWidth;

handle.addEventListener("mousedown", (e) => {
    resizing_left = true;
    startX = e.clientX;
    startWidth = parseInt(document.defaultView.getComputedStyle(el).width, 10);
    document.body.style.cursor = "e-resize";
    document.body.style.userSelect = "none";
    e.preventDefault();
});

window.addEventListener("mousemove", (e) => {
    if (!resizing_left) return;

    const width = startWidth + e.clientX - startX;
    const minWidth = 100; 
    const maxWidth = 300; 

    if (width >= minWidth && width <= maxWidth) {
        el.style.width = width + "px";
    }
});

window.addEventListener("mouseup", () => {
    resizing_left = false;
    document.body.style.cursor = "";
    document.body.style.userSelect = "";
});