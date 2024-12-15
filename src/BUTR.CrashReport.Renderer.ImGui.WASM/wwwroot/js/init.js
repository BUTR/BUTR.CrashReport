import { initializeModule } from './main.js'

const onDOMContentLoaded = async () => {
    const inputContainer = document.getElementById('input-container');
    const loader = document.getElementById('loader');
    const submitButton = document.getElementById('submit-button');
    const urlInput = document.getElementById('url-input');
    
    const urlParams = new URLSearchParams(window.location.search);
    const arg = urlParams.get('arg');

    if (arg) {
        document.removeEventListener('DOMContentLoaded', onDOMContentLoaded, false);
        await initializeModule(arg);
        return;
    }

    inputContainer.style.display = 'flex';
    loader.style.display = 'none';

    const onClick = () => {
        const userInput = urlInput.value.trim();
        if (userInput) {
            submitButton.removeEventListener('click', onClick);
            window.location.search = `?arg=${encodeURIComponent(userInput)}`;
        } else {
            alert('Please enter a valid URL.');
        }
    }
    submitButton.addEventListener('click', onClick);
};

document.addEventListener('DOMContentLoaded', onDOMContentLoaded, false);