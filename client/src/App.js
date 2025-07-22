// App.js
import { useState, useEffect } from 'react';
import './App.css';
import './Header.css';
import './Footer.css'

import Header from './Header';
import Footer from './Footer';
import DraggableGrid from './DraggableGrid';


function App() {
  const [darkMode, setDarkMode] = useState(false);

  useEffect(() => {
    document.body.classList.toggle('dark-mode', darkMode);
    document.body.classList.toggle('light-mode', !darkMode);
  }, [darkMode]);

  useEffect(() => {
    const prefersDark = window.matchMedia("(prefers-color-scheme: dark)").matches;
    setDarkMode(prefersDark);
  }, []);

  return (
    <div className={`app-container ${darkMode ? 'dark' : ''}`}>
      <Header username="My" darkMode={darkMode} setDarkMode={setDarkMode} />
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