export function init(elementId) {
    const el = document.getElementById(elementId);
    if (!el) return;
    
    // Only enable pan/zoom functionality on mobile devices (width < 1024px)
    if (window.innerWidth >= 1024) {
        // Explicitly remove constraints if they were added
        if (el.parentElement) {
            el.parentElement.style.overflow = '';
            el.parentElement.style.touchAction = '';
            el.parentElement.style.maxHeight = '';
            el.parentElement.style.width = '';
        }
        return;
    }
    
    // Add constraints for zooming
    if (el.parentElement) {
        el.parentElement.style.overflow = 'hidden';
        // Remove touchAction = 'none' so native scrolling can occur when we allow it
        el.parentElement.style.touchAction = 'auto';
        el.parentElement.style.maxHeight = '85vh';
        el.parentElement.style.width = '100%';
    }
    
    // Calculate initial zoom to fit BOTH width and height on mobile
    let parentWidth = el.parentElement.clientWidth || window.innerWidth;
    let parentHeight = el.parentElement.clientHeight || (window.innerHeight * 0.85);
    let elWidth = el.clientWidth || 1235;
    let elHeight = el.clientHeight || 1500;
    
    let widthRatio = (parentWidth - 10) / elWidth;
    let heightRatio = (parentHeight - 10) / elHeight;
    let startZoom = Math.min(widthRatio, heightRatio);
    
    // Destroy previous instance if it exists
    if (window._pzInstance) {
        window._pzInstance.dispose();
    }
    
    var pz = window.panzoom(el, {
        maxZoom: 5,
        minZoom: startZoom,
        bounds: false, // Turn off buggy native bounds
        boundsPadding: 0,
        initialZoom: startZoom,
        initialX: 0,
        initialY: 0,
        onTouch: function(e) {
            if (e.touches.length > 1) {
                e.preventDefault();
            }
            return false; 
        }
    });
    
    window._pzInstance = pz;

    // Rigidly enforce bounds using a continuous loop
    let isRunning = true;
    const enforceBounds = () => {
        if (!isRunning) return;
        
        var t = pz.getTransform();
        if (!t) return;
        
        var pW = el.parentElement.clientWidth;
        var rect = el.parentElement.getBoundingClientRect();
        var pH = window.innerHeight - rect.top; // Calculate exact visible height on screen
        if (pH < 200) pH = el.parentElement.clientHeight; // Fallback if something is weird
        
        var eW = el.clientWidth * t.scale;
        var eH = el.clientHeight * t.scale;
        
        var minX = pW - eW;
        var minY = pH - eH - 100; // Add 100px padding to the bottom so it's not tightly bound
        
        // If element is smaller than parent, center it.
        if (minX > 0) minX = minX / 2;
        if (minY > 0) minY = minY / 2;
        
        var newX = t.x;
        var newY = t.y;
        var changed = false;
        
        // Clamp X
        if (eW >= pW) {
            if (newX > 0) { newX = 0; changed = true; }
            if (newX < minX) { newX = minX; changed = true; }
        } else {
            if (Math.abs(newX - minX) > 1) { newX = minX; changed = true; }
        }
        
        // Clamp Y
        if (eH >= pH) {
            if (newY > 20) { newY = 20; changed = true; } // 20px overscroll at top
            if (newY < minY) { newY = minY; changed = true; }
        } else {
            // If it's smaller, don't rigidly lock it, allow panning within the safe zone
            if (newY > 20) { newY = 20; changed = true; }
            if (newY < minY) { newY = minY; changed = true; }
        }
        
        if (changed) {
            pz.moveTo(newX, newY);
        }
        
        requestAnimationFrame(enforceBounds);
    };
    
    requestAnimationFrame(enforceBounds);
    
    // Intercept touchmove to allow native scroll chaining when at boundaries
    let touchStartY = 0;
    let touchStartX = 0;
    
    const handleTouchStart = (e) => {
        if (e.touches.length === 1) {
            touchStartY = e.touches[0].clientY;
            touchStartX = e.touches[0].clientX;
        }
    };
    
    const handleTouchMove = (e) => {
        if (!window._pzInstance) return;
        if (e.touches.length > 1) return; // Let panzoom handle pinch
        
        let t = window._pzInstance.getTransform();
        let currentY = e.touches[0].clientY;
        let deltaY = currentY - touchStartY;
        
        var rect = el.parentElement.getBoundingClientRect();
        var pH = window.innerHeight - rect.top;
        if (pH < 200) pH = el.parentElement.clientHeight;
        var eH = el.clientHeight * t.scale;
        var minY = pH - eH - 100;
        
        // If we are at the top boundary and pulling down (deltaY > 0)
        if (t.y >= 20 && deltaY > 0) {
            e.stopImmediatePropagation();
            return;
        }
        
        // If we are at the bottom boundary and pulling up (deltaY < 0)
        if (t.y <= minY && deltaY < 0) {
            e.stopImmediatePropagation();
            return;
        }
        
        // Not at boundary: Prevent browser from scrolling the page
        if (e.cancelable) {
            e.preventDefault();
        }
    };
    
    el.addEventListener('touchstart', handleTouchStart, { passive: true });
    el.addEventListener('touchmove', handleTouchMove, { capture: true, passive: false });
    
    // Cleanup function attached to element to stop loop if element is removed
    el._stopBoundsLoop = () => { 
        isRunning = false; 
        el.removeEventListener('touchstart', handleTouchStart);
        el.removeEventListener('touchmove', handleTouchMove, { capture: true });
    };
}
