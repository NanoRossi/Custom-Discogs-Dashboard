import { useEffect, useState } from "react";

export default function ListEntries({ title, textBoxApiCall, listApiCall }) {
    const [options, setOptions] = useState([]);
    const [entries, setEntries] = useState([]);
    const [loading, setLoading] = useState(false);
    const [selected, setSelected] = useState("");

    const fetchOptions = async () => {
        try {
            const res = await fetch(`${process.env.REACT_APP_API_BASE_URL}/${textBoxApiCall}`);
            const data = await res.json();
            setOptions(data);
        } catch (err) {
            console.error(`Failed to fetch ${title}s`, err);
        }
    };

    const fetchEntires = async () => {

        setLoading(true);
        try {
            const res = await fetch(`${process.env.REACT_APP_API_BASE_URL}/${listApiCall}/${selected}`);
            const data = await res.json();
            setEntries(data);
        } catch (err) {
            console.error(`Failed to fetch ${title}s`, err);
        }

        setLoading(false);
    };

    useEffect(() => {
        fetchOptions();
    }, []);

    const handleChange = (e) => {
        setSelected(e.target.value);
        console.log('Selected:', e.target.value);
    };

    return (
        <div className="item-list-card">
            <h3 className="item-list-title">Get all for {title}</h3>

            <div className="dropdown-container">
                <select id={`options-${title}`} value={selected} onChange={handleChange} className="styled-select">
                    <option value="">-- Select --</option>
                    {options.map(item => (
                        <option key={item} value={item}>
                            {item}
                        </option>
                    ))}
                </select>

                <button onClick={(e) => { e.stopPropagation(); fetchEntires(); }} disabled={loading || selected === ""} className="get-button">
                    {loading ? "Loading..." : "Get All!"}
                </button>
            </div>

            <div className="item-list-scroll">
                {entries.map((item, index) => (
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