import { useState, useEffect } from "react";

export default function GetFact() {
    const [fact, setFact] = useState(null);
    const [fadeIn, setFadeIn] = useState(false);
    const [fadeOut, setFadeOut] = useState(false);

    useEffect(() => {
        const fetchFact = async () => {
            // Start fade out
            setFadeOut(true);
            setFadeIn(false);
            await new Promise((resolve) => setTimeout(resolve, 500));

            try {
                const apiBaseUrl = window._env_?.REACT_APP_API_BASE_URL || "http://localhost:8001";
                const res = await fetch(`${apiBaseUrl}/api/info/fact`);
                const data = await res.text();

                setFact(data);
                setFadeOut(false);
                setFadeIn(true);
            } catch (err) {
                console.error(`Failed to fetch fact`, err);
            }
        };

        fetchFact();

        // Set up interval to call it every 30 seconds
        const intervalId = setInterval(fetchFact, 30000); // 30000 ms = 30 sec

        // Clean up on unmount
        return () => clearInterval(intervalId);
    }, []);


    return (
        <div className="item-list-card">
            <h3 className="item-list-title">By The Numbers</h3>
            <div className={`fact-container`}>
                <div className={`fact-text-container fade ${fadeIn ? "visible" : ""} ${fadeOut ? "" : "visible"}`}>
                    <span>{fact}</span>
                </div>
            </div>
        </div>
    );
}
