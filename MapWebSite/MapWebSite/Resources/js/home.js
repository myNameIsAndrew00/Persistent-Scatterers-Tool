var loadedScripts = [];

function getScript(node, scriptServerPath) {    
    if (loadedScripts[scriptServerPath] === true) return;

    loadedScripts[scriptServerPath] = true;
        
    var script = document.createElement('script');
    script.src = scriptServerPath;
    $(node).append(script);
}