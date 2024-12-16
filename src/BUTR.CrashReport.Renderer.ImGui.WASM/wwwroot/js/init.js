import { initializeModule } from './main.js'

const onDOMContentLoaded = async () => {
    const inputContainer = document.getElementById('input-container');
    const loader = document.getElementById('loader');
    
    const urlInput = document.getElementById('url-input');
    const fileInput = document.getElementById('file-input');
    const submitButton = document.getElementById('submit-button');
    
    const urlParams = new URLSearchParams(window.location.search);
    const arg = urlParams.get('arg');

    const processCrashReportFromURL = async (url) => {
        document.removeEventListener('DOMContentLoaded', onDOMContentLoaded, false);

        inputContainer.style.display = 'none';
        loader.style.display = 'flex';
        
        await initializeModule('JsonLink', url, null);
    }

    const processCrashReportFromFile = async (type, fileContent) => {
        document.removeEventListener('DOMContentLoaded', onDOMContentLoaded, false);

        inputContainer.style.display = 'none';
        loader.style.display = 'flex';
        
        await initializeModule(type, null, fileContent);
    }
    
    if (arg) {
        await processCrashReportFromURL(arg);
        return;
    }

    inputContainer.style.display = 'flex';
    loader.style.display = 'none';
    
    const onClick = async () => {
        const url = urlInput.value.trim();
        const file = fileInput.files[0];

        if (url) {
            submitButton.removeEventListener('click', onClick);
            await processCrashReportFromURL(url);
        } else if (file) {
            submitButton.removeEventListener('click', onClick);
            const reader = new FileReader();
            reader.onload = async (event) => {
                let type = '';
                if (file.name.endsWith('.json')) {
                    type = 'JsonFile';
                }
                if (file.name.endsWith('.zip')) {
                    type = 'ZipFile';
                }
                const fileContent = new Uint8Array(event.target.result);
                await processCrashReportFromFile(type, fileContent);
            };
            reader.readAsArrayBuffer(file);
        } else {
            alert('Please enter a valid URL or provide a file.');
        }
    }
    submitButton.addEventListener('click', onClick);
};

document.addEventListener('DOMContentLoaded', onDOMContentLoaded, false);