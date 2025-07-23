import { useEffect, useState } from "react";
import "../css/ListEntries.css";

export default function ListEntries({ title, apiCall }) {
    const [wantlist, setWantlist] = useState([]);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const res = await fetch(`${process.env.REACT_APP_API_BASE_URL}/${apiCall}`);
                const data = await res.json();
                setWantlist(data);
            } catch (err) {
                console.error(`Failed to fetch ${title}`, err);
            }
        };

        fetchData();
    }, []);

    return (
        <div className="item-list-card">
            <h3 className="item-list-title">{title}</h3>
            <div className="item-list-scroll">
                {wantlist.map((item, index) => (
                    <div className="item-list-entry" key={index}>
                        <img src={item.thumbnail} alt={`${item.artistName} - ${item.releaseName}`} />
                        <div>
                            <strong>{item.artistName}</strong><br />
                            <span>{item.releaseName}</span><br />
                            <span>{item.formatType}</span>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}