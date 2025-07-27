import { useState, useEffect } from "react";
import { DndContext, closestCenter, PointerSensor, useSensor, useSensors } from "@dnd-kit/core";
import { arrayMove, SortableContext, useSortable, rectSortingStrategy } from "@dnd-kit/sortable";
import { restrictToParentElement } from '@dnd-kit/modifiers';
import { CSS } from "@dnd-kit/utilities";
import { FaGripLines } from "react-icons/fa";

import "../css/DraggableGrid.css";
import "../css/GridItems.css";
import ListEntries from "./grid-components/ListEntries";
import GetItem from "./grid-components/GetItem";
import GetStatus from "./grid-components/GetStatus";
import GetAllFor from "./grid-components/GetAllFor";
import GetFact from "./grid-components/GetFact";

function SortableItem({ id, content }) {
    const { attributes, listeners, setNodeRef, transform, transition } =
        useSortable({ id });

    const style = {
        transform: CSS.Transform.toString(transform),
        transition,
    };

    return (
        <div ref={setNodeRef} className="sortable-item" style={style} {...attributes}>
            <div className="drag-handle" {...listeners} style={{ cursor: "grab", padding: 4 }}>
                <FaGripLines />
            </div>
            <div className="sortable-content">{content}</div>
        </div>
    );
}

export default function DraggableGrid() {
    const defaultItems = [
        { id: "1", content: <ListEntries title={"Wantlist"} apiCall={"api/wantlist"} /> },
        { id: "2", content: <ListEntries title={"Recent Additions"} apiCall={"api/collection/recent/10"} /> },
        { id: "3", content: <GetItem title={"Get Random Record"} apiCall={"api/collection/random/vinyl"} /> },
        { id: "4", content: <GetItem title={"Get Random CD"} apiCall={"api/collection/random/cd"} /> },
        { id: "5", content: <GetFact /> },
        { id: "6", content: <GetStatus /> },
        { id: "7", content: <GetAllFor title={"Artist"} textBoxApiCall={"api/info/artists"} listApiCall={"api/collection/getall/artist"} /> },
        { id: "8", content: <GetAllFor title={"Genre"} textBoxApiCall={"api/info/genres"} listApiCall={"api/collection/getall/genre"} /> },
        { id: "9", content: <GetAllFor title={"Style"} textBoxApiCall={"api/info/styles"} listApiCall={"api/collection/getall/style"} /> }
    ];

    const [items, setItems] = useState(defaultItems);

    useEffect(() => {
        const savedOrder = localStorage.getItem("grid-order");
        if (savedOrder) {
            const order = JSON.parse(savedOrder);
            const orderedItems = order
                .map((id) => defaultItems.find((item) => item.id === id))
                .filter(Boolean);
            const missingItems = defaultItems.filter(
                (item) => !orderedItems.some((oi) => oi.id === item.id)
            );
            setItems([...orderedItems, ...missingItems]);
        }
    }, []);

    // Sensors for drag-and-drop
    const sensors = useSensors(useSensor(PointerSensor));

    function handleDragEnd(event) {
        const { active, over } = event;
        if (active.id !== over?.id) {
            setItems((items) => {
                const oldIndex = items.findIndex((item) => item.id === active.id);
                const newIndex = items.findIndex((item) => item.id === over.id);
                const newOrder = arrayMove(items, oldIndex, newIndex);
                localStorage.setItem(
                    "grid-order",
                    JSON.stringify(newOrder.map((item) => item.id))
                );
                return newOrder;
            });
        }
    }

    return (
        <div>
            <DndContext
                sensors={sensors}
                collisionDetection={closestCenter}
                onDragEnd={handleDragEnd}
                modifiers={[restrictToParentElement]}
            >
                <SortableContext items={items.map(item => item.id)} strategy={rectSortingStrategy}>
                    <div className="draggable-grid">
                        {items.map(({ id, content }) => (
                            <SortableItem key={id} id={id} content={content} />
                        ))}
                    </div>
                </SortableContext>
            </DndContext>
        </div>
    );
}
