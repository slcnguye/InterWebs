ko.extenders.scrollFollow = function(target, selector) {
    target.subscribe(function (newval) {
        var el = document.querySelector(selector);

        // the scroll bar is all the way down, so we know they want to follow the text
        if (el.scrollTop == el.scrollHeight - el.clientHeight) {
            // have to push our code outside of this thread since the text hasn't updated yet
            setTimeout(function() { el.scrollTop = el.scrollHeight - el.clientHeight; }, 0);
        }
    });

    return target;
}