import { dotnet } from './_framework/dotnet.js'
import { BrotliDecode } from './decode.min.js';

let loadedCount = 0;
let totalCount = 0;

const motivationalMessages = [
    "Fetching resources, hang tight...",
    "Almost there, stay with us!",
    "Preparing magic...",
    "Final touches being applied..."
];

// DOM Elements
const canvas = document.getElementById("canvas");
const loader = document.getElementById('loader');
const spinner = document.getElementById('spinner');
const progressBarContainer = document.getElementById('progress-bar-container');
const progressBar = document.getElementById('progress-bar');
const progressLabel = document.getElementById('progress-label');
const fileNameElement = document.getElementById('file-name');
const motivationalText = document.getElementById('motivational-text');
let isFinalizing = false; // Tracks whether the spinner is active

const resetLoader = () => {
    isFinalizing = false; // Reset finalization state

    // Reset the progress bar and spinner
    progressBar.style.width = `0%`;
    progressBarContainer.style.opacity = 1;
    progressBarContainer.style.display = 'block';
    spinner.style.opacity = 0;
    spinner.style.display = 'none';

    // Reset text elements
    progressLabel.innerText = "Downloading...";
    fileNameElement.innerText = "Loading resources...";
    motivationalText.innerText = "Hang tight, preparing magic...";
};

const updateLoader = (resourceName) => {
    const percentLoaded = 10 + (loadedCount * 90.0) / totalCount;

    // If finalizing and new data is starting to download, reset the loader
    if (isFinalizing && loadedCount < totalCount) {
        resetLoader();
    }

    // Update progress bar width
    progressBar.style.width = `${percentLoaded}%`;
    progressBarContainer.setAttribute('aria-valuenow', percentLoaded);

    // Update the progress label inside the bar
    progressLabel.innerText = `Downloading ${loadedCount}/${totalCount}`;

    // Update the file name below the bar
    fileNameElement.innerText = resourceName || "Loading resources...";

    // Update motivational text below everything
    const messageIndex = Math.min(
        Math.floor((percentLoaded / 100) * motivationalMessages.length),
        motivationalMessages.length - 1
    );
    motivationalText.innerText = motivationalMessages[messageIndex];

    // Ensure spinner is hidden while downloading
    if (!isFinalizing && percentLoaded > 10) {
        spinner.style.opacity = 0;
        spinner.style.display = 'none';
        progressBarContainer.style.display = 'block';
    }

    // Handle finalization when downloads complete
    if (loadedCount === totalCount && !isFinalizing) {
        isFinalizing = true; // Mark as finalizing
        progressLabel.innerText = "Finalizing...";
        fileNameElement.innerText = "Initializing the Crash Reporter Renderer...";
        motivationalText.innerText = "Almost ready!";

        // Transition to spinner
        setTimeout(() => {
            progressBarContainer.style.opacity = 0; // Fade out progress bar
            setTimeout(() => {
                progressBarContainer.style.display = 'none'; // Hide progress bar
                spinner.style.display = 'block'; // Show spinner
                setTimeout(() => {
                    spinner.style.opacity = 1; // Fade in spinner
                }, 50); // Ensure spinner visibility before opacity transition
            }, 500); // Wait for fade-out to complete
        }, 300); // Smooth transition delay
    }
};

const { setModuleImports, runMain } = await dotnet
    .withResourceLoader((type, name, defaultUri, integrity, behavior) => {
        if (type === 'dotnetjs') {
            return defaultUri;
        }

        totalCount++;

        return (async () => {
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

            // Update progress
            loadedCount++;
            updateLoader(name);

            return response;
        })();
    })
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

dotnet.instance.Module.canvas = canvas;

setModuleImports('main.js', {
    finishedLoading: () => {
        loader.style.display = 'none'; // Hide loader when finished
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
            // Fallback logic
        }
    },
});

await runMain();