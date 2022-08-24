import { useState, useEffect } from "react";
import { Stage, Layer, RegularPolygon, Rect, Text, Group, Image } from "react-konva";
import routerIcon from "../../../assets/images/internet-router.png";
import beaconIcon from "../../../assets/images/hotspot-tower.png";

const Site = ({ x, y, topW, leftH }) => {
  return (
    <Rect
      x={x}
      y={y}
      width={topW}
      height={leftH}
      strokeWidth={1}
      stroke={"rgba(0, 0, 0, 0.2)"}
      fill={"rgba(0, 0, 0, 0.2)"}
      cornerRadius={20}
    />
  );
};

const CanvasSite = ({ width = 1680, height = 780, site, selectedSite }) => {
  const [stageScale, setStageScale] = useState(1);
  const [stageX, setStageX] = useState(0);
  const [stageY, setStageY] = useState(0);

  const [beaconImage, setBeaconImage] = useState(new window.Image());

  useEffect(() => {
    const img = new window.Image();
    img.src = beaconIcon;
    setBeaconImage(img);
  }, []);

  const initialState = selectedSite.in.map((e, i) => ({
    id: e.beacon.macAddress,
    name: e.beacon.name,
    x: Math.random() * 12,
    y: Math.random() * 3,
    isDragging: false,
  }));
  const [items, setItems] = useState(initialState);

  const handleDragStart = (e) => {
    const id = e.target.id();
    setItems(
      items.map((item) => {
        return {
          ...item,
          isDragging: item.id === id,
        };
      })
    );
  };
  const handleDragEnd = (e) => {
    setItems(
      items.map((item) => {
        return {
          ...item,
          isDragging: false,
        };
      })
    );
  };

  function renderSite() {
    const elements = [];
    elements.push(<Site key={`site_${site.id}`} x={0} y={0} topW={width} leftH={height} />);

    items.map((star) => {
      elements.push(
        <Group
          key={star.id}
          id={star.id}
          x={star.x * 121 + 60 * (star.y % 2)}
          y={star.y * 105}
          draggable
          onDragStart={handleDragStart}
          onDragEnd={handleDragEnd}
        >
          <RegularPolygon
            radius={70}
            sides={6}
            width={100}
            height={100}
            fill="#89b717"
            stroke="#89b717"
            strokeWidth={1}
            opacity={0.8}
            shadowColor="black"
            shadowBlur={10}
            shadowOpacity={0.6}
            shadowOffsetX={star.isDragging ? 10 : 5}
            shadowOffsetY={star.isDragging ? 10 : 5}
            scaleX={star.isDragging ? 1.2 : 1}
            scaleY={star.isDragging ? 1.2 : 1}
          />
          <Image x={-35} y={-25} width={70} height={50} image={beaconImage} />
          <Text x={-38} y={68} text={`${star.id}\n${star.name}`}></Text>
        </Group>
      );
    });
    return elements;
  }

  function handleWheel(e) {
    e.evt.preventDefault();

    const stage = e.target.getStage();
    const oldScale = stage.scaleX();
    const mousePointTo = {
      x: stage.getPointerPosition().x / oldScale - stage.x() / oldScale,
      y: stage.getPointerPosition().y / oldScale - stage.y() / oldScale,
    };

    const deltaYBounded = !(e.evt.deltaY % 1)
      ? Math.abs(Math.min(-10, Math.max(10, e.evt.deltaY)))
      : Math.abs(e.evt.deltaY);

    const scaleBy = 1.01 + deltaYBounded / 150;
    const newScale = e.evt.deltaY > 0 ? oldScale / scaleBy : oldScale * scaleBy;

    stage.scale({ x: newScale, y: newScale });

    stage.position({
      x: -(mousePointTo.x - stage.getPointerPosition().x / newScale) * newScale,
      y: -(mousePointTo.y - stage.getPointerPosition().y / newScale) * newScale,
    });
    stage.batchDraw();
  }

  return (
    <Stage
      height={height}
      onWheel={handleWheel}
      scaleX={stageScale}
      scaleY={stageScale}
      x={stageX}
      y={stageY}
      draggable={true}
      width={width}
    >
      <Layer>{renderSite()}</Layer>
    </Stage>
  );
};

export default CanvasSite;
