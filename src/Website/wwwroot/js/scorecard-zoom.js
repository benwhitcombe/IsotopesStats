export function init(elementId) {
    const el = document.getElementById(elementId);
    if (!el) return;
    
    // Only enable pan/zoom on mobile devices
    if (window.isMobileDevice && !window.isMobileDevice()) {
        if (window._pzInstance) {
            window._pzInstance.dispose();
            window._pzInstance = null;
        }
        if (el.parentElement) {
            el.parentElement.style.overflow = '';
            el.parentElement.style.touchAction = '';
            el.parentElement.style.width = '';
            el.parentElement.style.maxHeight = '';
        }
        // Remove transform if it was previously applied
        el.style.transform = '';
        el.style.transformOrigin = '';
        return;
    }
    
    // Add constraints for zooming
    if (el.parentElement) {
        el.parentElement.style.overflow = 'hidden';
        // Remove touchAction = 'none' so native scrolling can occur when we allow it
        el.parentElement.style.touchAction = 'auto';
        el.parentElement.style.width = '100%';
        if (window.innerWidth < 1024) {
            el.parentElement.style.maxHeight = '85vh';
        } else {
            el.parentElement.style.maxHeight = 'none'; // Allow native page scrolling on desktop
        }
    }
    
    // Calculate initial zoom
    let parentWidth = el.parentElement.clientWidth || window.innerWidth;
    let parentHeight = el.parentElement.clientHeight || (window.innerHeight * 0.85);
    let elWidth = el.offsetWidth || 1235;
    let elHeight = el.offsetHeight || 1500;
    
    let widthRatio = (parentWidth - 10) / elWidth;
    let heightRatio = (parentHeight - 10) / elHeight;
    
    let startZoom = Math.min(widthRatio, heightRatio);
    if (window.innerWidth >= 1024) {
        // On desktop, don't force fitting the height to keep it readable,
        // and limit the max zoom to 1.0 (actual size).
        startZoom = Math.min(widthRatio, 1);
    }
    
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
        },
        beforeWheel: function(e) {
            if (!window._pzInstance) return false;
            var t = window._pzInstance.getTransform();
            
            // If the user scrolls down (e.deltaY > 0) to zoom out, but we're already fully zoomed out,
            // ignore the wheel event in panzoom to let the browser natively scroll the page down.
            if (e.deltaY > 0 && t.scale <= startZoom + 0.001) {
                return true; 
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
        var pH = el.parentElement.clientHeight; 
        if (window.innerWidth < 1024) {
            pH = window.innerHeight - rect.top; // Calculate exact visible height on screen for mobile
            if (pH < 200) pH = el.parentElement.clientHeight; // Fallback if something is weird
        }
        
        var eW = el.offsetWidth * t.scale;
        var eH = el.offsetHeight * t.scale;
        
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
        var pH = el.parentElement.clientHeight;
        if (window.innerWidth < 1024) {
            pH = window.innerHeight - rect.top;
            if (pH < 200) pH = el.parentElement.clientHeight;
        }
        var eH = el.offsetHeight * t.scale;
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
