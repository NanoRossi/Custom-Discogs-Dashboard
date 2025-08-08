import { useEffect, useState } from "react";

export default function GetStatus() {
    const [status, setStatus] = useState([]);
    const [loading, setLoading] = useState(false);


    useEffect(() => {
        const getStatus = async () => {
            try {
                const apiBaseUrl = window._env_?.REACT_APP_API_BASE_URL || "http://localhost:8001";
                const res = await fetch(`${apiBaseUrl}/api/status`);
                const data = await res.json();
                setStatus(data);
            } catch (err) {
                console.error(`Failed to fetch status`, err);
            }
        };

        setLoading(false);
        getStatus();
    }, []);

    const refreshData = async () => {
        setLoading(true);
        try {
            const apiBaseUrl = window._env_?.REACT_APP_API_BASE_URL || "http://localhost:8001";
            await fetch(`${apiBaseUrl}/api/import`);
            window.location.reload();
        } catch (err) {
            console.error(`Failed to refresh`, err);
        }
    };

    return (
        <div className="item-list-card">
            <h3 className="item-list-title">Database Info</h3>

            <div className="status">
                <span>
                    Database Status:{" "}
                    <span className={`status-${(status.databaseStatus || "Disconnected").toLowerCase()}`}>
                        {status.databaseStatus || "Disconnected"}
                    </span>
                </span><br />
                <span>{status.collectionCount} items in the collection</span><br />
                <span>{status.wantlistCount} on the wantlist</span><br />
                <span>{status.genreCount} different genres</span><br />
                <span>{status.styleCount} different styles</span><br />
                <span>{status.vinylCount} Records</span><br />
                <span>{status.cdCount} CDs</span><br />
                <span>{status.cassetteCount} Cassettes</span><br />
            </div>

            <button onClick={(e) => { e.stopPropagation(); refreshData(); }} disabled={loading} className="fetch-button">
                {loading ? "Loading..." : "Refresh Database"}
            </button>
        </div>
    );
}