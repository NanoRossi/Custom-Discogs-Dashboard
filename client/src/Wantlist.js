import { useEffect, useState } from "react";
import "./Wantlist.css";

export default function WantlistItem() {
    const [wantlist, setWantlist] = useState([]);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const res = await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/wantlist`);
                const data = await res.json();
                setWantlist(data);
            } catch (err) {
                console.error("Failed to fetch wantlist", err);
            }
        };

        fetchData();
    }, []);

    return (
        <div className="wantlist-card">
            <h3 className="wantlist-title">Wantlist</h3>
            <div className="wantlist-scroll">
                {wantlist.map((item, index) => (
                    <div className="wantlist-entry" key={index}>
                        <img src={item.thumbnail} alt={`${item.artistName} - ${item.releaseName}`} />
                        <div>
                            <strong>{item.artistName}</strong><br />
                            <span>{item.releaseName}</span>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}