window.getWindowWidth = function () {
    return window.innerWidth;
};

window.setBodyScroll = function (allowScroll) {
    if (allowScroll) {
        document.body.style.overflow = '';
    } else {
        document.body.style.overflow = 'hidden';
    }
};

window.drawerDragging = {
    isDragging: false,
    drawerElement: null,
    isExpanded: false,
    dotNetHelper: null,
    threshold: 40,
    collapsedTranslate: 0,
    initialized: false,
    currentDelta: 0,
    startX: 0,
    startY: 0,
    initialTranslate: 0,
    isCarousel: false,
    dragDirectionDetected: false,

    init: function (dotNetHelper, initialExpanded) {
        this.dotNetHelper = dotNetHelper;
        this.isExpanded = initialExpanded;
        
        if (!this.initialized) {
            this.setupListeners();
            this.initialized = true;
        }
        
        this.sync();
    },

    setupListeners: function() {
        window.addEventListener('touchstart', (e) => {
            const isDrawer = e.target.closest('.mobile-stat-drawer');
            const isGrid = e.target.closest('.stat-grid-full');
            const carousel = e.target.closest('.stat-carousel');

            // Drag works on anything in the drawer except the vertically scrollable grid
            // This includes handle, title, and the peek area
            if (isDrawer && !isGrid) {
                this.handleTouchStart(e, !!carousel);
            }
        }, { passive: true });

        window.addEventListener('touchmove', (e) => {
            if (this.isDragging) {
                this.handleTouchMove(e);
            }
        }, { passive: false });

        window.addEventListener('touchend', (e) => {
            if (this.isDragging) {
                this.handleTouchEnd(e);
            }
        });

        window.addEventListener('resize', () => this.sync());
    },

    sync: function() {
        this.drawerElement = document.getElementById('stat-drawer');
        const handle = document.getElementById('drawer-handle');
        const carousel = this.drawerElement ? this.drawerElement.querySelector('.stat-carousel') : null;

        if (!this.drawerElement || !handle || !carousel) return;

        const fullHeight = this.drawerElement.offsetHeight;
        const visibleHeight = handle.offsetHeight + carousel.offsetHeight;
        this.collapsedTranslate = fullHeight - visibleHeight;

        if (!this.isDragging) {
            this.applyStateTransform(true);
        }
    },

    updateState: function(isExpanded) {
        this.isExpanded = isExpanded;
        this.applyStateTransform(false);
        
        if (!isExpanded) {
            // Give CSS transition time to finish before scrolling
            setTimeout(() => this.scrollToActive(), 400);
        }
    },

    scrollToActive: function(immediate) {
        // Use requestAnimationFrame to ensure the browser has performed layout
        requestAnimationFrame(() => {
            const carousel = document.querySelector('.stat-carousel');
            if (!carousel) return;
            
            const activeBtn = carousel.querySelector('.stat-btn.active');
            if (!activeBtn) return;

            // If dimensions aren't ready yet, try once more in the next frame
            if (carousel.offsetWidth === 0 || activeBtn.offsetWidth === 0) {
                requestAnimationFrame(() => this.scrollToActive(immediate));
                return;
            }

            // Check if already fully visible to prevent disorienting jumps
            const isVisible = (activeBtn.offsetLeft >= carousel.scrollLeft) && 
                              (activeBtn.offsetLeft + activeBtn.offsetWidth <= carousel.scrollLeft + carousel.offsetWidth);

            if (isVisible) return;

            const scrollLeft = activeBtn.offsetLeft - (carousel.offsetWidth / 2) + (activeBtn.offsetWidth / 2);
            
            if (immediate) {
                carousel.scrollLeft = scrollLeft;
            } else {
                carousel.scrollTo({
                    left: scrollLeft,
                    behavior: 'smooth'
                });
            }
        });
    },

    applyStateTransform: function(immediate) {
        if (!this.drawerElement) return;
        this.drawerElement.style.transition = immediate ? 'none' : 'transform 0.4s cubic-bezier(0.16, 1, 0.3, 1)';
        const targetTranslate = this.isExpanded ? 0 : this.collapsedTranslate;
        this.drawerElement.style.transform = `translateY(${targetTranslate}px)`;
    },

    handleTouchStart: function (e, isCarousel) {
        this.drawerElement = document.getElementById('stat-drawer');
        if (!this.drawerElement) return;

        this.startX = e.touches[0].clientX;
        this.startY = e.touches[0].clientY;
        this.isDragging = true;
        this.isCarousel = isCarousel;
        this.dragDirectionDetected = false;
        this.drawerElement.style.transition = 'none';
        this.currentDelta = 0;
        this.initialTranslate = this.isExpanded ? 0 : this.collapsedTranslate;
    },

    handleTouchMove: function (e) {
        if (!this.isDragging || !this.drawerElement) return;

        const touchX = e.touches[0].clientX;
        const touchY = e.touches[0].clientY;
        
        // If we're on the carousel, we need to distinguish between horizontal scroll and vertical drag
        if (this.isCarousel && !this.dragDirectionDetected) {
            const deltaX = Math.abs(touchX - this.startX);
            const deltaY = Math.abs(touchY - this.startY);
            
            if (deltaX > 10 || deltaY > 10) {
                if (deltaX > deltaY) {
                    // It's a horizontal swipe, let the carousel scroll naturally
                    this.isDragging = false;
                    return;
                } else {
                    // It's a vertical swipe, proceed with drawer dragging
                    this.dragDirectionDetected = true;
                }
            } else {
                return; // Wait for more movement to be sure
            }
        }

        this.currentDelta = touchY - this.startY;
        let newTranslate = this.initialTranslate + this.currentDelta;

        if (newTranslate < 0) {
            newTranslate = 0; 
        } else if (newTranslate > this.collapsedTranslate) {
            const extra = newTranslate - this.collapsedTranslate;
            newTranslate = this.collapsedTranslate + (extra * 0.2);
        }

        this.drawerElement.style.transform = `translateY(${newTranslate}px)`;
        
        // Block page scroll while we are actually dragging the drawer
        if (e.cancelable) {
            e.preventDefault();
        }
    },

    handleTouchEnd: function (e) {
        if (!this.isDragging) return;
        this.isDragging = false;
        
        const movedUp = this.currentDelta < -this.threshold;
        const movedDown = this.currentDelta > this.threshold;

        if (movedUp && !this.isExpanded) {
            this.isExpanded = true;
            this.dotNetHelper.invokeMethodAsync('SetDrawerState', true);
        } else if (movedDown && this.isExpanded) {
            this.isExpanded = false;
            this.dotNetHelper.invokeMethodAsync('SetDrawerState', false);
        }
        
        this.applyStateTransform(false);
    },
    
    // Utility to swap full names for short names when they wrap
    initOpponentNames: function() {
        if (!window.ResizeObserver) return;

        if (!this.nameObserver) {
            this.nameObserver = new ResizeObserver(entries => {
                for (let entry of entries) {
                    const container = entry.target;
                    const full = container.querySelector('.full-name');
                    const short = container.querySelector('.short-name');
                    
                    if (!full || !short) continue;

                    // If elements aren't fully rendered, skip
                    if (container.clientWidth === 0 || full.scrollWidth === 0) continue;

                    // Check if the full name's natural width exceeds the container's available width
                    const isWrapped = full.scrollWidth > container.clientWidth;
                    
                    if (isWrapped) {
                        container.classList.add('use-short-name');
                    } else {
                        container.classList.remove('use-short-name');
                    }
                }
            });
        }

        // Observe all smart name containers that aren't already being observed
        document.querySelectorAll('.smart-name-container').forEach(el => {
            this.nameObserver.observe(el);
        });
    }
};
