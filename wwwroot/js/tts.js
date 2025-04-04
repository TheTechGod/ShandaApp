let selectedVoice = null;

// ✅ Speak Greeting w/ Voice Preference (e.g. "female", "zira", etc.)
function speakGreeting(text, preferredVoice = "female") {
    const synth = window.speechSynthesis;

    function loadVoicesAndSpeak() {
        const voices = synth.getVoices();

        if (!voices.length) {
            console.error("No TTS voices available.");
            return;
        }

        // 🔍 Match preferred voice
        selectedVoice = voices.find(v =>
            v.name.toLowerCase().includes(preferredVoice.toLowerCase())
        );

        // 🔁 Fallback to any female voice
        if (!selectedVoice) {
            selectedVoice = voices.find(v =>
                v.name.toLowerCase().includes("female")
            );
        }

        // 🔁 Fallback to first available voice
        if (!selectedVoice) {
            selectedVoice = voices[0];
        }

        console.log("✅ Using voice:", selectedVoice.name);

        const utterance = new SpeechSynthesisUtterance(text);
        utterance.voice = selectedVoice;
        synth.speak(utterance);
    }

    if (!synth.getVoices().length) {
        synth.onvoiceschanged = loadVoicesAndSpeak;
    } else {
        loadVoicesAndSpeak();
    }
}

// ✅ Speak arbitrary text (after voice initialized)
function speakText(text) {
    if (!text || !window.speechSynthesis) return;

    const utterance = new SpeechSynthesisUtterance(text);
    utterance.voice = selectedVoice;
    utterance.rate = 1;
    utterance.pitch = 1;

    speechSynthesis.cancel();
    speechSynthesis.speak(utterance);
}
