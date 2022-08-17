import { useState, useEffect } from "react";
import { Stage, Layer, Circle, RegularPolygon, Image, Rect } from "react-konva";
import routerIcon from "assets/images/internet-router.png";

function generateShapes(width, height) {
  return [...Array(10)].map((_, i) => ({
    id: i.toString(),
    x: Math.random() * width,
    y: Math.random() * height,
    isDragging: false,
  }));
}

const CANVAS_VIRTUAL_WIDTH = 1000;
const CANVAS_VIRTUAL_HEIGHT = 1000;

const WarehouseCanvas = ({ width, height }) => {
  const INITIAL_STATE = generateShapes(width, height);
  const [stars, setStars] = useState(INITIAL_STATE);
  const [image, setImage] = useState(new window.Image());

  useEffect(() => {
    const img = new window.Image();
    img.src = routerIcon;
    setImage(img);
  }, []);

  const handleDragStart = (e) => {
    const id = e.target.id();
    setStars(
      stars.map((star) => {
        return {
          ...star,
          isDragging: star.id === id,
        };
      })
    );
  };
  const handleDragEnd = (e) => {
    setStars(
      stars.map((star) => {
        return {
          ...star,
          isDragging: false,
        };
      })
    );
  };

  const scale = Math.min(width / CANVAS_VIRTUAL_WIDTH, height / CANVAS_VIRTUAL_HEIGHT);
  console.log("scale", scale);
  return (
    <Stage width={width} height={height} scaleX={scale} scaleY={scale}>
      <Layer>
        <Rect
          key={"site1"}
          id={"site1"}
          x={0}
          y={0}
          width={width / scale}
          height={height / scale}
          stroke={"rgba(0, 0, 0, 0.2)"}
        />
        {/* <Image x={50} y={50} width={50} height={50} image={image} />*/}
        {stars.map((star) => (
          <RegularPolygon
            key={star.id}
            id={star.id}
            x={star.x}
            y={star.y}
            //radius={50}
            sides={6}
            width={100}
            height={100}
            //fill="#89b717"
            stroke="black"
            opacity={0.8}
            draggable
            shadowColor="black"
            shadowBlur={10}
            shadowOpacity={0.6}
            shadowOffsetX={star.isDragging ? 10 : 5}
            shadowOffsetY={star.isDragging ? 10 : 5}
            scaleX={star.isDragging ? 1.2 : 1}
            scaleY={star.isDragging ? 1.2 : 1}
            onDragStart={handleDragStart}
            onDragEnd={handleDragEnd}
          />
        ))}
      </Layer>
    </Stage>
  );
};

export default WarehouseCanvas;
