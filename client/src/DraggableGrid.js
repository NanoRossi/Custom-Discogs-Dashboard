import { useState } from "react";
import {
    DndContext,
    closestCenter,
    PointerSensor,
    useSensor,
    useSensors,
} from "@dnd-kit/core";
import {
    arrayMove,
    SortableContext,
    useSortable,
    rectSortingStrategy,
} from "@dnd-kit/sortable";

import {
    restrictToParentElement
} from '@dnd-kit/modifiers';

import { CSS } from "@dnd-kit/utilities";

import WantlistItem from "./Wantlist";
import "./DraggableGrid.css";

function SortableItem({ id, content }) {
    const { attributes, listeners, setNodeRef, transform, transition } =
        useSortable({ id });

    const style = {
        transform: CSS.Transform.toString(transform),
        transition,
    };

    return (
        <div
            ref={setNodeRef}
            className="sortable-item"
            style={style}
            {...attributes}
            {...listeners}
        >
            {content}
        </div>
    );
}

export default function DraggableGrid() {
    const [items, setItems] = useState([
        { id: "1", content: <WantlistItem /> },
        { id: "2", content: "Result Item 2" },
        { id: "3", content: "Result Item 3" },
        { id: "4", content: "Result Item 4" },
        { id: "5", content: "Result Item 5" },
        { id: "6", content: "Result Item 6" },
    ]);

    // Sensors for drag-and-drop
    const sensors = useSensors(useSensor(PointerSensor));

    function handleDragEnd(event) {
        const { active, over } = event;
        if (active.id !== over?.id) {
            setItems((items) => {
                const oldIndex = items.findIndex((item) => item.id === active.id);
                const newIndex = items.findIndex((item) => item.id === over.id);
                return arrayMove(items, oldIndex, newIndex);
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
                <SortableContext items={items} strategy={rectSortingStrategy}>
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
