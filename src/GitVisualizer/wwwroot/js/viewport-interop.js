// viewport-interop.js — mobile breakpoint detection and resize subscription
// Breakpoint: mobile = < 768px
let _dotnetRef = null;
let _mql = null;

function handleChange(e) {
    _dotnetRef?.invokeMethodAsync('OnBreakpointChanged', e.matches);
}

/** Returns true when viewport width is below the 768px mobile breakpoint. */
export function isMobile() {
    return window.innerWidth < 768;
}

/**
 * Subscribes a .NET DotNetObjectReference to receive breakpoint-change callbacks.
 * Fires OnBreakpointChanged(bool isMobile) when the viewport crosses 768px.
 */
export function subscribeResize(dotnetRef) {
    _dotnetRef = dotnetRef;
    _mql = window.matchMedia('(max-width: 767px)');
    _mql.addEventListener('change', handleChange);
}

/** Removes the resize listener and releases the .NET reference. */
export function unsubscribeResize() {
    if (_mql) {
        _mql.removeEventListener('change', handleChange);
        _mql = null;
    }
    _dotnetRef = null;
}
