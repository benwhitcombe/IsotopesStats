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
        el.parentElement.style.touchAction = 'none';
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
        var pH = el.parentElement.clientHeight;
        var eW = el.clientWidth * t.scale;
        var eH = el.clientHeight * t.scale;
        
        var minX = pW - eW;
        var minY = pH - eH;
        
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
            if (newY > 0) { newY = 0; changed = true; }
            if (newY < minY) { newY = minY; changed = true; }
        } else {
            if (Math.abs(newY - minY) > 1) { newY = minY; changed = true; }
        }
        
        if (changed) {
            pz.moveTo(newX, newY);
        }
        
        requestAnimationFrame(enforceBounds);
    };
    
    requestAnimationFrame(enforceBounds);
    
    // Cleanup function attached to element to stop loop if element is removed
    el._stopBoundsLoop = () => { isRunning = false; };
}
