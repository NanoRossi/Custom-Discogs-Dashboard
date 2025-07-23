import { useState, useEffect } from "react";
import "../css/GetItem.css";

export default function GetItem({ title, apiCall }) {
    const [item, setItem] = useState(null);
    const [loading, setLoading] = useState(false);
    const [fadeIn, setFadeIn] = useState(false);
    const [fadeOut, setFadeOut] = useState(false);

    const fetchData = async () => {
        setLoading(true);

        if (item) {
            // Start fade out
            setFadeOut(true);
            setFadeIn(false);
            await new Promise((resolve) => setTimeout(resolve, 1000));
        }

        try {
            const res = await fetch(`${process.env.REACT_APP_API_BASE_URL}/${apiCall}`);
            const data = await res.json();
            // Preload image
            const img = new Image();
            img.src = data.coverImage;

            img.onload = () => {
                setItem(data);
                setFadeOut(false);
                setFadeIn(true);
            };

            img.onerror = () => {
                console.error("Image failed to load:", data.coverImage);
                setItem(data);
                setFadeOut(false);
                setFadeIn(true);
            };
        } catch (err) {
            console.error(`Failed to fetch ${title}`, err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (!loading && item) {
            const timeout = setTimeout(() => setFadeIn(true), 500);
            return () => clearTimeout(timeout);
        }
    }, [loading, item]);

    return (
        <div className="item-list-card">
            <h3 className="item-list-title">{title}</h3>
            <div className={`item-container ${item ? "visible" : ""}`}>
                {item && (
                    <div className={`item-entry fade ${fadeIn ? "visible" : ""} ${fadeOut ? "" : "visible"}`}>
                        <img
                            src={item.coverImage}
                            alt={`${item.artistName} - ${item.releaseName}`}
                        />
                        <div>
                            <strong>{item.artistName}</strong><br />
                            <span>{item.releaseName}</span>
                        </div>
                    </div>
                )}
            </div>

            <button onClick={(e) => { e.stopPropagation(); fetchData(); }} disabled={loading} className="fetch-button">
                {loading ? "Loading..." : "Get Music!"}
            </button>
        </div>
    );
}
