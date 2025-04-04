import { dotnet } from './../_framework/dotnet.js'
import { BrotliDecode } from './decode.min.js';
import { hideLoader, setResourcesToTrack, updateLoader} from './loader.js';

const canvas = document.getElementById("canvas");

const getResponsePromise = async (defaultUri, type) => {
    let response = null;

    if (response === null) {
        let responseBrotli = await fetch(`${defaultUri}.br`, { cache: 'no-cache' });
        if (responseBrotli.ok){
            const originalResponseBuffer = await responseBrotli.arrayBuffer();
            const originalResponseArray = new Int8Array(originalResponseBuffer);
            const decompressedResponseArray = BrotliDecode(originalResponseArray);
            const contentType = type === 'dotnetwasm' ? 'application/wasm' : 'application/octet-stream';
            response = new Response(decompressedResponseArray, {headers: {'content-type': contentType}});
        }
    }

    if (response === null) {
        let responseGzipped = await fetch(`${defaultUri}.gz`, { cache: 'no-cache' });
        if (responseGzipped.ok) {
            const ds = new DecompressionStream("gzip");
            const decompressedStream = responseGzipped.body.pipeThrough(ds);
            const contentType = type === 'dotnetwasm' ? 'application/wasm' : 'application/octet-stream';
            response = new Response(decompressedStream, {headers: {'content-type': contentType}});
        }
    }

    if (response === null) {
        response = await fetch(defaultUri, { cache: 'no-cache' });
    }
    
    return response;
};

export const initializeModule = async (type, url, data) => {
    const moduleImports = {
        getArgType: () => {
            return type;
        },
        getArgUrl: () => {
            return url;
        },
        getArgData: () => {
            return data;
        },
        isDarkMode: () => {
            return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
        },
        finishedLoading: () => {
            hideLoader();
        },
        saveFile: (data, filename) => {
            const blob = new Blob([data], {type: 'application/octet-stream'});
            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = filename;
            a.click();
            URL.revokeObjectURL(url);
        },
        writeClipboard: (data) => {
            if (navigator.clipboard && navigator.clipboard.writeText) {
                return navigator.clipboard.writeText(data);
            } else {
                console.warn('Clipboard API not available');
            }
        },
    };
    
    
    const jsonResponse = await fetch('./../_framework/blazor.boot.json');
    const boot = await jsonResponse.json();
    setResourcesToTrack(Object.keys(boot.resources?.fingerprinting || {}));
    
    const cacheMatchOriginal = Cache.prototype.match;
    Cache.prototype.match = async function (request, options) {
        const response = await cacheMatchOriginal.call(this, request, options);
        if (typeof request === 'string') {
            updateLoader(request);
        }
        return response;
    };
    
    const created = await dotnet
        .withResourceLoader((type, name, defaultUri, integrity, behavior) => {
            if (type === 'dotnetjs') {
                return defaultUri;
            }

            updateLoader(name);

            return getResponsePromise(defaultUri, type);
        })
        .withDiagnosticTracing(false)
        .withApplicationArgumentsFromQuery()
        .create();
    
    Cache.prototype.match = cacheMatchOriginal;

    const { Module, config, setModuleImports, getAssemblyExports, runMain } = created;
    
    Module.canvas = canvas;
    setModuleImports('interop', moduleImports);

    const exports = await getAssemblyExports(config.mainAssemblyName);
    const setDarkMode = exports.BUTR?.CrashReport?.Renderer?.ImGui.WASM?.Program?.SetDarkMode || ((_) => {});

    window.matchMedia("(prefers-color-scheme: dark)").addEventListener("change", (e) => {
        if (e.matches) {
            document.body.dataset.theme = "dark";
            setDarkMode(true);
        } else {
            document.body.dataset.theme = "light";
            setDarkMode(false);
        }
    });
    
    await runMain();
}