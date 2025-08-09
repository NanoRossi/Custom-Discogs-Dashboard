import { useEffect, useState } from "react";
import { fetchWithTimeout } from '../utils/fetchWithTimeout';

export default function ListEntries({ title, apiCall }) {
    const [wantlist, setWantlist] = useState([]);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const apiBaseUrl = window._env_?.REACT_APP_API_BASE_URL || "http://localhost:8001";
                const res = await fetchWithTimeout(`${apiBaseUrl}/${apiCall}`, {}, 2000);
                const data = await res.json();
                setWantlist(data);
            } catch (err) {
                console.error(`Failed to fetch ${title}`, err);
            }
        };

        fetchData();
    }, [title, apiCall]);

    return (
        <div className="item-list-card">
            <h3 className="item-list-title">{title}</h3>
            <div className="item-list-scroll">
                {wantlist.map((item, index) => (
                    <div className="item-list-entry" key={index}>
                        <img src={item.thumbnail} alt={`${item.artistName.join(', ')} - ${item.releaseName}`} />
                        <div>
                            <strong>{item.artistName.join(', ')}</strong><br />
                            <span>{item.releaseName}</span><br />
                            <span>
                                {item.formatInfo && item.formatInfo.discInfo && item.formatInfo?.discInfo.some(d => d.text) ? (
                                    item.formatInfo.discInfo
                                        .filter(d => d.text)
                                        .map((d, idx, arr) => (
                                            <span key={d.id}>
                                                {d.quantity} × {d.text}
                                                {idx < arr.length - 1 && ', '} Vinyl
                                            </span>
                                        ))
                                ) : (
                                    item.formatInfo?.formatType
                                )}
                            </span>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}