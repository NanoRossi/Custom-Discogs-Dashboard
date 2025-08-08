// App.js
import { useState, useEffect } from 'react';
import '../css/App.css';

import Header from './Header';
import Footer from './Footer';
import DraggableGrid from './DraggableGrid';

// Custom hook to save dark mode state to localStorage
// TODO, probably move this to a separate file
function useLocalStorageState(key) {
  const [state, setState] = useState(() => {
    const storedValue = localStorage.getItem(key);
    if (storedValue !== null) {
      return (storedValue === 'true');
    }
    return false;;
  });

  // Update localStorage whenever state changes
  useEffect(() => {
    localStorage.setItem(key, state);
  }, [key, state]);

  return [state, setState];
}


function App() {
  const [darkMode, setDarkMode] = useState(false);
  const [savedDarkMode, setSavedDarkMode] = useLocalStorageState('darkMode', false);

  useEffect(() => {
    document.body.classList.toggle('dark-mode', darkMode);
    document.body.classList.toggle('light-mode', !darkMode);
  }, [darkMode]);

  useEffect(() => {
    const prefersDark = window.matchMedia("(prefers-color-scheme: dark)").matches;

    // if there is a saved dark mode preference, use it
    // otherwise, use the system preference - this will only happen on the first load
    if (savedDarkMode !== null) {
      setDarkMode(savedDarkMode);
    }
    else {
      setDarkMode(prefersDark);
    }
  }, [savedDarkMode]);

  return (
    <div className={`app-container ${darkMode ? 'dark' : ''}`}>
      <Header darkMode={darkMode} setDarkMode={setDarkMode} setSavedDarkMode={setSavedDarkMode} />
      <div className="content-container">
        <div className="main-content">
          <DraggableGrid />
          <Footer />
        </div>
      </div>
    </div>
  );
}

export default App;