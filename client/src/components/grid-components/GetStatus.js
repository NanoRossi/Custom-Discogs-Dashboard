import { useEffect, useState } from "react";
import "../../css/ListEntries.css";
import "../../css/GetStatus.css";

export default function GetStatus() {
    const [status, setStatus] = useState([]);

    useEffect(() => {
        const getStatus = async () => {
            try {
                const res = await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/status`);
                const data = await res.json();
                setStatus(data);
            } catch (err) {
                console.error(`Failed to fetch status`, err);
            }
        };

        getStatus();
    }, []);

    return (
        <div className="item-list-card">
            <h3 className="item-list-title">Database Stats:</h3>

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
            </div>
        </div>
    );
}