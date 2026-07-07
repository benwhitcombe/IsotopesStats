window.getUserAgent = function () {
    return window.navigator ? window.navigator.userAgent : 'unknown';
};

window.isMobileDevice = function () {
    try {
        const ua = window.navigator ? window.navigator.userAgent : '';
        return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(ua);
    } catch (e) {
        return false;
    }
};

// Add a class to the body for CSS targeting based on device type
document.addEventListener("DOMContentLoaded", function() {
    if (!window.isMobileDevice()) {
        document.body.classList.add('desktop-device');
    }
});

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
        const prevState = this.isExpanded;
        this.isExpanded = isExpanded;
        
        if (isExpanded && !prevState) {
            this.scrollToActive(true);
        }

        this.applyStateTransform(false);
        setTimeout(() => this.scrollToActive(), 400);
    },

    scrollToActive: function(immediate) {
        requestAnimationFrame(() => {
            const carousel = document.querySelector('.stat-carousel');
            if (carousel && (!this.isExpanded || immediate)) {
                const activeBtn = carousel.querySelector('.stat-btn.active');
                if (activeBtn) {
                    if (carousel.offsetWidth > 0 && activeBtn.offsetWidth > 0) {
                        const isVisible = (activeBtn.offsetLeft >= carousel.scrollLeft) && 
                                          (activeBtn.offsetLeft + activeBtn.offsetWidth <= carousel.scrollLeft + carousel.offsetWidth);

                        if (!isVisible || immediate) {
                            const scrollLeft = activeBtn.offsetLeft - (carousel.offsetWidth / 2) + (activeBtn.offsetWidth / 2);
                            if (immediate) {
                                carousel.scrollLeft = scrollLeft;
                            } else {
                                carousel.scrollTo({ left: scrollLeft, behavior: 'smooth' });
                            }
                        }
                    }
                }
            }

            const grid = document.querySelector('.stat-grid-full');
            if (grid && (this.isExpanded || immediate)) {
                const activeBtn = grid.querySelector('.stat-btn.active');
                if (activeBtn) {
                    if (grid.offsetHeight > 0 && activeBtn.offsetHeight > 0) {
                        const isVisible = (activeBtn.offsetTop >= grid.scrollTop) && 
                                          (activeBtn.offsetTop + activeBtn.offsetHeight <= grid.scrollTop + grid.offsetHeight);

                        if (!isVisible || immediate) {
                            const scrollTop = activeBtn.offsetTop - (grid.offsetHeight / 2) + (activeBtn.offsetWidth / 2);
                            if (immediate) {
                                grid.scrollTop = scrollTop;
                            } else {
                                grid.scrollTo({ top: scrollTop, behavior: 'smooth' });
                            }
                        }
                    } else if (immediate) {
                        requestAnimationFrame(() => this.scrollToActive(true));
                    }
                }
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
        
        if (this.isCarousel && !this.dragDirectionDetected) {
            const deltaX = Math.abs(touchX - this.startX);
            const deltaY = Math.abs(touchY - this.startY);
            
            if (deltaX > 10 || deltaY > 10) {
                if (deltaX > deltaY) {
                    this.isDragging = false;
                    return;
                } else {
                    this.dragDirectionDetected = true;
                }
            } else {
                return;
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
        
        if (e.cancelable) {
            e.preventDefault();
        }
    },

    handleTouchEnd: function (e) {
        if (!this.isDragging) return;
        this.isDragging = false;
        
        const movedUp = this.currentDelta < -this.threshold;
        const movedDown = this.currentDelta > this.threshold;
        const prevState = this.isExpanded;

        if (movedUp && !this.isExpanded) {
            this.isExpanded = true;
            this.dotNetHelper.invokeMethodAsync('SetDrawerState', true);
            this.scrollToActive(true);
        } else if (movedDown && this.isExpanded) {
            this.isExpanded = false;
            this.dotNetHelper.invokeMethodAsync('SetDrawerState', false);
        }
        
        this.applyStateTransform(false);

        if (this.isExpanded !== prevState) {
            setTimeout(() => this.scrollToActive(), 400);
        }
    },
    
    initOpponentNames: function() {
        if (!window.ResizeObserver) return;

        if (!this.nameObserver) {
            this.nameObserver = new ResizeObserver(entries => {
                for (let entry of entries) {
                    const container = entry.target;
                    const full = container.querySelector('.full-name');
                    const short = container.querySelector('.short-name');
                    
                    if (!full || !short) continue;
                    if (container.clientWidth === 0 || full.scrollWidth === 0) continue;
                    const isWrapped = full.scrollWidth > container.clientWidth;
                    
                    if (isWrapped) {
                        container.classList.add('use-short-name');
                    } else {
                        container.classList.remove('use-short-name');
                    }
                }
            });
        }

        document.querySelectorAll('.smart-name-container').forEach(el => {
            this.nameObserver.observe(el);
        });
    }
};

let lastDragEvent = null;
window.addEventListener('dragstart', (e) => {
    lastDragEvent = e;
}, true);

window.setDragImage = function (elementId, offsetX, offsetY) {
    const el = document.getElementById(elementId);
    const ev = window.event || lastDragEvent;
    if (el && ev && ev.dataTransfer) {
        try {
            ev.dataTransfer.setDragImage(el, offsetX, offsetY);
        } catch (err) {
            console.error('setDragImage error:', err);
        }
    }
};

window.initDrag = function () {
    const ev = window.event;
    if (ev && ev.dataTransfer) {
        try {
            ev.dataTransfer.setData('text/plain', 'drag');
            ev.dataTransfer.effectAllowed = 'move';
            ev.dataTransfer.dropEffect = 'move';
        } catch (err) {
            console.error('initDrag error:', err);
        }
    }
};

window.handleDragOver = function () {
    const ev = window.event;
    if (ev && ev.dataTransfer) {
        ev.dataTransfer.dropEffect = 'move';
    }
};

window.updateUrlQuery = function (key, value) {
    const url = new URL(window.location.href);
    if (value) {
        url.searchParams.set(key, value);
    } else {
        url.searchParams.delete(key);
    }
    window.history.replaceState(null, '', url.toString());
};

window.setDragHighlight = function (elementId, className, add) {
    const el = document.getElementById(elementId);
    if (!el) return;
    if (add) {
        el.classList.add(className);
    } else {
        el.classList.remove(className);
    }
};

window.clearAllDragHighlights = function (className) {
    document.querySelectorAll('.' + className).forEach(el => el.classList.remove(className));
};

window.shareOrCopy = async function (title, url) {
    let shareUrl = url;
    if (!shareUrl || shareUrl === 'null') {
        shareUrl = window.location.href;
    }
    
    const isMobile = window.isMobileDevice();
    
    if (navigator.share && isMobile) {
        try {
            const shareData = { url: shareUrl };
            if (title && title !== 'null') {
                shareData.title = title;
            }
            await navigator.share(shareData);
            return { shared: true };
        } catch (err) {
            if (err.name === 'AbortError') {
                return { shared: false, cancelled: true };
            }
            return { shared: false, error: err.message };
        }
    } else {
        try {
            await navigator.clipboard.writeText(shareUrl);
            return { copied: true };
        } catch (err) {
            return { copied: false, error: err.message };
        }
    }
};

window.autoPositionDropdown = function (wrapper) {
    if (!wrapper) return;
    const dropdown = wrapper.querySelector('.suggestions-dropdown, .pos-suggestions-dropdown');
    if (!dropdown) return;

    const rect = wrapper.getBoundingClientRect();
    const spaceBelow = window.innerHeight - rect.bottom;
    const spaceAbove = rect.top;
    const preferredHeight = 250;

    if (spaceBelow < preferredHeight && spaceAbove > spaceBelow) {
        dropdown.classList.add('open-up');
        dropdown.style.maxHeight = (Math.min(preferredHeight, spaceAbove) - 10) + 'px';
    } else {
        dropdown.classList.remove('open-up');
        dropdown.style.maxHeight = (Math.min(preferredHeight, spaceBelow) - 10) + 'px';
    }
};

window.initLineupSortable = function (tbodyElement, dotNetHelper) {
    if (!tbodyElement || typeof Sortable === 'undefined') return;
    
    if (tbodyElement.sortableInstance) {
        tbodyElement.sortableInstance.destroy();
    }

    const container = tbodyElement.closest('.lineup-content-wrapper');

    const clearHighlights = () => {
        if (container) {
            container.querySelectorAll('.drag-over, .drag-over-pos, .drag-target').forEach(el => {
                el.classList.remove('drag-over', 'drag-over-pos', 'drag-target');
            });
        }
    };

    const getInteractionTarget = (e) => {
        const x = e.clientX || (e.touches && e.touches[0].clientX);
        const y = e.clientY || (e.touches && e.touches[0].clientY);
        if (!x || !y) return null;

        // Hide fallback/ghost to see what's under
        const fallback = document.querySelector('.sortable-fallback');
        const customGhost = document.getElementById('custom-drag-ghost');
        const prevFallbackDisplay = fallback ? fallback.style.display : null;
        const prevGhostDisplay = customGhost ? customGhost.style.display : null;
        
        if (fallback) fallback.style.display = 'none';
        if (customGhost) customGhost.style.display = 'none';
        
        const el = document.elementFromPoint(x, y);
        
        if (fallback) fallback.style.display = prevFallbackDisplay;
        if (customGhost) customGhost.style.display = prevGhostDisplay;

        if (!el) return null;

        return {
            fieldBox: el.closest('.player-overlay'),
            benchBox: el.closest('.bench-player-name'),
            benchContainer: el.closest('.bench-container'),
            posCell: el.closest('.pos-abbr'),
            row: el.closest('tr.reorderable-row'),
            el: el
        };
    };

    const globalMoveHandler = (e) => {
        clearHighlights();
        const targets = getInteractionTarget(e);
        if (!targets) return;

        if (tbodyElement._isPosDrag) {
            if (targets.posCell && targets.row && !targets.row.classList.contains('empty-row')) {
                targets.posCell.classList.add('drag-over-pos');
            } else if (targets.fieldBox) {
                targets.fieldBox.classList.add('drag-over');
            } else if (targets.benchBox) {
                targets.benchBox.classList.add('drag-over');
            } else if (targets.benchContainer) {
                targets.benchContainer.classList.add('drag-over');
            }
        } else {
            if (targets.row) targets.row.classList.add('drag-over');
            else if (targets.fieldBox) targets.fieldBox.classList.add('drag-over');
            else if (targets.benchBox) {
                targets.benchBox.classList.add('drag-over');
            } else if (targets.benchContainer) {
                targets.benchContainer.classList.add('drag-over');
            }
        }
    };

    // --- TABLE SORTABLE ---
    tbodyElement.sortableInstance = new Sortable(tbodyElement, {
        animation: 0,
        handle: '.pos-num, .player-select-cell, .pos-abbr',
        draggable: 'tr:not(.empty-row)',
        ghostClass: 'dragging-hidden',
        chosenClass: 'chosen',
        filter: '.clear-btn, .suggestions-dropdown, .native-player-select',
        preventOnFilter: false,
        fallbackTolerance: 5,
        forceFallback: true,
        fallbackClass: 'sortable-fallback',
        fallbackOnBody: true,
        sort: false,
        onStart: function (evt) {
            const handle = evt.originalEvent.target;
            const isPosDrag = !!handle.closest('.pos-abbr');
            tbodyElement._isDragging = true;
            tbodyElement._isPosDrag = isPosDrag;
            tbodyElement.classList.add('is-dragging-row');
            if (isPosDrag) tbodyElement.classList.add('is-dragging-pos');

            const table = tbodyElement.closest('table');
            if (table) table.classList.add('is-dragging');
            
            if (document.activeElement && typeof document.activeElement.blur === 'function') {
                document.activeElement.blur();
            }

            // Sync fallback style
            requestAnimationFrame(() => {
                const fallback = document.querySelector('.sortable-fallback');
                if (fallback) {
                    fallback.style.pointerEvents = 'none';
                    fallback.style.zIndex = '50000';
                    const originalRow = evt.item;
                    const rect = originalRow.getBoundingClientRect();
                    fallback.style.width = rect.width + 'px';
                    fallback.style.height = rect.height + 'px';
                    fallback.style.display = 'table';
                    fallback.style.tableLayout = 'fixed';
                    
                    const originalCells = originalRow.querySelectorAll('td');
                    const fallbackCells = fallback.querySelectorAll('td');
                    for (let i = 0; i < originalCells.length; i++) {
                        if (fallbackCells[i]) {
                            const cellRect = originalCells[i].getBoundingClientRect();
                            fallbackCells[i].style.width = cellRect.width + 'px';
                            fallbackCells[i].style.height = cellRect.height + 'px';
                        }
                    }

                    if (isPosDrag) {
                        fallback.classList.add('fallback-pos-only');
                    }
                }
            });

            window.addEventListener('mousemove', globalMoveHandler);
            window.addEventListener('touchmove', globalMoveHandler, { passive: false });
        },
        onEnd: function (evt) {
            window.removeEventListener('mousemove', globalMoveHandler);
            window.removeEventListener('touchmove', globalMoveHandler);

            const isPosDrag = tbodyElement._isPosDrag;
            tbodyElement._isDragging = false;
            tbodyElement._isPosDrag = false;
            tbodyElement.classList.remove('is-dragging-row', 'is-dragging-pos');
            const table = tbodyElement.closest('table');
            if (table) table.classList.remove('is-dragging');

            const targets = getInteractionTarget(evt.originalEvent);
            clearHighlights();

            const oldIndex = evt.oldIndex;
            
            if (isPosDrag) {
                if (targets && targets.posCell && targets.row && !targets.row.classList.contains('empty-row')) {
                    const newIndex = Array.from(tbodyElement.children).indexOf(targets.row);
                    if (newIndex !== -1) dotNetHelper.invokeMethodAsync('OnPosSwapped', oldIndex, newIndex);
                } else if (targets && (targets.fieldBox || targets.benchBox || targets.benchContainer)) {
                    const fieldIdx = targets.fieldBox ? parseInt(targets.fieldBox.id.split('-').pop()) : null;
                    const fromBench = !!(targets.benchBox || targets.benchContainer);
                    dotNetHelper.invokeMethodAsync('OnTableDroppedOnField', oldIndex, fieldIdx, fromBench);
                }
            } else {
                if (targets && targets.row) {
                    const newIndex = Array.from(tbodyElement.children).indexOf(targets.row);
                    if (oldIndex !== newIndex && newIndex !== -1) {
                        dotNetHelper.invokeMethodAsync('OnRowReordered', oldIndex, newIndex);
                    }
                } else if (targets && (targets.fieldBox || targets.benchBox || targets.benchContainer)) {
                    const fieldIdx = targets.fieldBox ? parseInt(targets.fieldBox.id.split('-').pop()) : null;
                    const fromBench = !!(targets.benchBox || targets.benchContainer);
                    dotNetHelper.invokeMethodAsync('OnTableDroppedOnField', oldIndex, fieldIdx, fromBench);
                }
            }
        }
    });

        // --- FIELD INTERACTIONS ---
    if (container) {
        let fieldDragData = null;
        let ghost = null;
        let preventNextClick = false;

        const handleFieldStart = (el, e, index, fromBench) => {
            // Only allow dragging if we are not clicking a remove button
            if (e.target.classList.contains('remove-icon')) return;

            const startX = e.clientX || (e.touches && e.touches[0].clientX);
            const startY = e.clientY || (e.touches && e.touches[0].clientY);

            fieldDragData = { el, index, fromBench, startX, startY, moved: false };
            
            window.addEventListener('mousemove', handleFieldMove);
            window.addEventListener('mouseup', handleFieldEnd);
            window.addEventListener('touchmove', handleFieldMove, { passive: false });
            window.addEventListener('touchend', handleFieldEnd);
        };

        const updateGhostPosition = (e) => {
            if (!ghost) return;
            const x = e.clientX || (e.touches && e.touches[0].clientX);
            const y = e.clientY || (e.touches && e.touches[0].clientY);
            ghost.style.left = (x - ghost.offsetWidth / 2) + 'px';
            ghost.style.top = (y - ghost.offsetHeight / 2) + 'px';
        };

        const handleFieldMove = (e) => {
            if (!fieldDragData) return;
            const x = e.clientX || (e.touches && e.touches[0].clientX);
            const y = e.clientY || (e.touches && e.touches[0].clientY);

            if (!fieldDragData.moved) {
                const dist = Math.sqrt(Math.pow(x - fieldDragData.startX, 2) + Math.pow(y - fieldDragData.startY, 2));
                if (dist > 5) {
                    fieldDragData.moved = true;
                    
                    document.body.classList.add('is-dragging-field');
                    
                    // Create ghost
                    const sourceEl = fieldDragData.el;
                    ghost = sourceEl.cloneNode(true);
                    ghost.id = 'custom-drag-ghost';
                    ghost.style.position = 'fixed';
                    ghost.style.pointerEvents = 'none';
                    ghost.style.zIndex = '50000';
                    ghost.style.opacity = '0.8';
                    ghost.style.width = sourceEl.offsetWidth + 'px';
                    ghost.style.height = sourceEl.offsetHeight + 'px';
                    
                    // For field boxes, make the ghost look like a player-name-box
                    if (!fieldDragData.fromBench) {
                        const nameBox = ghost.querySelector('.player-name-box');
                        if (nameBox) {
                            ghost.innerHTML = '';
                            ghost.appendChild(nameBox);
                            nameBox.style.transform = 'none';
                            nameBox.style.position = 'static';
                        }
                    }

                    document.body.appendChild(ghost);
                } else {
                    return;
                }
            }

            updateGhostPosition(e);
            if (e.cancelable) e.preventDefault();

            clearHighlights();
            const targets = getInteractionTarget(e);
            if (targets) {
                if (targets.fieldBox) targets.fieldBox.classList.add('drag-over');
                else if (targets.benchBox || targets.benchContainer) {
                    const bc = targets.benchContainer || targets.benchBox.closest('.bench-container');
                    if (bc) bc.classList.add('drag-over');
                }
                else if (targets.posCell) targets.posCell.classList.add('drag-over-pos');
                else if (targets.row) targets.row.classList.add('drag-over');
            }
        };

        const handleFieldEnd = (e) => {
            if (!fieldDragData) return;
            
            if (fieldDragData.moved) {
                // IT WAS A DRAG
                preventNextClick = true;
                setTimeout(() => preventNextClick = false, 100);

                const targets = getInteractionTarget(e);
                clearHighlights();

                if (targets) {
                    if (targets.fieldBox || targets.benchBox || targets.benchContainer) {
                        const targetIdx = targets.fieldBox ? parseInt(targets.fieldBox.id.split('-').pop()) : null;
                        const targetFromBench = !!(targets.benchBox || targets.benchContainer);
                        
                        dotNetHelper.invokeMethodAsync('OnFieldSwap', 
                            fieldDragData.index, fieldDragData.fromBench, 
                            targetIdx, targetFromBench);
                    } else if (targets.posCell || targets.row) {
                        const rowIdx = Array.from(tbodyElement.children).indexOf(targets.row);
                        if (rowIdx !== -1) {
                            dotNetHelper.invokeMethodAsync('OnFieldDroppedOnTable', 
                                fieldDragData.index, fieldDragData.fromBench, rowIdx);
                        }
                    }
                }
            }

            // Cleanup
            document.body.classList.remove('is-dragging-field');
            if (ghost) {
                ghost.remove();
                ghost = null;
            }
            fieldDragData = null;
            window.removeEventListener('mousemove', handleFieldMove);
            window.removeEventListener('mouseup', handleFieldEnd);
            window.removeEventListener('touchmove', handleFieldMove);
            window.removeEventListener('touchend', handleFieldEnd);
        };

        // Suppress native click if a drag just ended
        container.addEventListener('click', (e) => {
            if (preventNextClick) {
                e.stopPropagation();
                e.preventDefault();
            }
        }, true);

        // Attach listeners to field boxes and bench names
        container.addEventListener('mousedown', (e) => {
            const fieldBox = e.target.closest('.player-overlay');
            if (fieldBox) {
                const idx = parseInt(fieldBox.id.split('-').pop());
                handleFieldStart(fieldBox, e, idx, false);
                return;
            }
            const benchBox = e.target.closest('.bench-player-name');
            if (benchBox) {
                const idx = parseInt(benchBox.id.split('-').pop());
                handleFieldStart(benchBox, e, idx, true);
                return;
            }
        });

        // Touch support
        container.addEventListener('touchstart', (e) => {
            const fieldBox = e.target.closest('.player-overlay');
            if (fieldBox) {
                const idx = parseInt(fieldBox.id.split('-').pop());
                handleFieldStart(fieldBox, e, idx, false);
                return;
            }
            const benchBox = e.target.closest('.bench-player-name');
            if (benchBox) {
                const idx = parseInt(benchBox.id.split('-').pop());
                handleFieldStart(benchBox, e, idx, true);
                return;
            }
        }, { passive: true });
    }
};
