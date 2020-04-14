var source = window.source = new EventSource("http://localhost:30120/live_map/sse");

source.onmessage =  function(e){
    console.log("TEST EVENT", e);
};

source.onopen = function (e) {
    console.log("EVENTSOURCE OPENED", e);
};

source.onerror = function (e) {
    console.log("error", e);
};

