let loadedResources = new Set();
let totalResources = new Set();
let isFinalizing = false;

const motivationalMessages = [
    "Fetching resources, hang tight...",
    "Almost there, stay with us!",
    "Preparing magic...",
    "Final touches being applied..."
];

const progressBarContainer = document.getElementById('progress-bar-container');
const progressBar = document.getElementById('progress-bar');
const progressLabel = document.getElementById('progress-label');
const fileNameElement = document.getElementById('file-name');
const motivationalText = document.getElementById('motivational-text');
const loader = document.getElementById('loader');

const resetLoader = () => {
    isFinalizing = false; // Reset finalization state

    // Reset the progress bar and spinner
    progressBar.style.width = `0%`;
    progressBarContainer.style.opacity = 1;
    progressBarContainer.style.display = 'block';

    // Reset text elements
    progressLabel.innerText = "Downloading...";
    fileNameElement.innerText = "Loading resources...";
    motivationalText.innerText = "Hang tight, preparing magic...";
};

export const hideLoader = () => {
    loader.style.display = 'none';
}

export const setResourcesToTrack = (newResources) => {
    for (const resource of newResources) {
        if (resource === 'dotnet.js' || /dotnet\.runtime\..*\.js/.test(resource) || /dotnet\.native\..*\.js/.test(resource)) {
            continue;
        }
        totalResources.add(resource);
    }
};

export const updateLoader = (resourceName) => {
    const resource = Array.from(totalResources).find(r => resourceName.includes(r));
    if (resource) {
        if (loadedResources.has(resource)) {
            return;
        }
        loadedResources.add(resource);
    }

    const percentLoaded = 10 + (loadedResources.size * 90.0) / totalResources.size;

    // If finalizing and new data is starting to download, reset the loader
    if (isFinalizing && loadedResources.size < totalResources.size) {
        resetLoader();
    }

    // Update progress bar width
    progressBar.style.width = `${percentLoaded}%`;
    progressBarContainer.setAttribute('aria-valuenow', percentLoaded.toString());

    // Update the progress label inside the bar
    progressLabel.innerText = `Downloading ${loadedResources.size}/${totalResources.size}`;

    // Update the file name below the bar
    fileNameElement.innerText = resource || resourceName || "Loading resources...";

    // Update motivational text below everything
    const messageIndex = Math.min(
        Math.floor((percentLoaded / 100) * motivationalMessages.length),
        motivationalMessages.length - 1
    );
    motivationalText.innerText = motivationalMessages[messageIndex];

    // Ensure spinner is hidden while downloading
    if (!isFinalizing && percentLoaded > 10) {
        progressBarContainer.style.display = 'block';
    }

    // Handle finalization when downloads complete
    if (loadedResources.size === totalResources.size && !isFinalizing) {
        isFinalizing = true; // Mark as finalizing
        progressLabel.innerText = "Finalizing...";
        fileNameElement.innerText = "Initializing the Crash Reporter Renderer...";
        motivationalText.innerText = "Almost ready!";

        setTimeout(() => {
            progressBarContainer.style.opacity = 0; // Fade out progress bar
            setTimeout(() => {
                progressBarContainer.style.display = 'none'; // Hide progress bar
            }, 500); // Wait for fade-out to complete
        }, 300); // Smooth transition delay
    }
};