import { useState, useEffect } from "react";
import {
  Stage,
  Layer,
  RegularPolygon,
  Rect,
  Text,
  Group,
  Image,
  Shape,
  Arrow,
  Line,
} from "react-konva";
import Konva from "konva";
import routerIcon from "assets/images/internet-router.png";

const Site = ({ x, y, topW, leftH }) => {
  const tW = topW * 72 + 2;
  const lH = leftH * 72 + 2;
  return (
    <Group x={x} y={y}>
      <Rect
        width={tW}
        height={lH}
        strokeWidth={2}
        stroke={"rgb(131,146,171)"}
        //fill={"rgba(0, 0, 0, 0.1)"}
        //cornerRadius={20}
      />
      <Arrow
        points={[x + 5, y - 20, tW - 4, y - 20]}
        pointerAtBeginning="true"
        pointerLength={30}
        pointerWidth={15}
        fill="black"
        stroke="black"
        opacity={0.2}
        strokeWidth={2}
      />
      <Line
        points={[x + 2, y, x + 2, -40]}
        stroke="black"
        tension={1}
        pointerLength={10}
        pointerWidth={12}
        opacity={0.2}
        strokeWidth={1}
      />
      <Line
        points={[tW, y, tW, -40]}
        stroke="black"
        tension={1}
        pointerLength={10}
        pointerWidth={12}
        opacity={0.2}
        strokeWidth={1}
      />
      <Text
        fill="red"
        x={tW / 2 - 25}
        y={y - 12}
        width={50}
        height={10}
        align="center"
        verticalAlign="middle"
        text={`${topW} m`}
        opacity={0.8}
      />
      <Arrow
        points={[x - 20, y + 5, x - 20, lH - 4]}
        pointerAtBeginning="true"
        pointerLength={30}
        pointerWidth={15}
        fill="black"
        stroke="black"
        opacity={0.2}
        strokeWidth={2}
      />
      <Line
        points={[x, y + 2, x - 40, y + 2]}
        stroke="black"
        tension={1}
        pointerLength={10}
        pointerWidth={12}
        opacity={0.2}
        strokeWidth={1}
      />
      <Line
        points={[x, lH, x - 40, lH]}
        stroke="black"
        tension={1}
        pointerLength={10}
        pointerWidth={12}
        opacity={0.2}
        strokeWidth={1}
      />
      <Text
        fill="red"
        x={x - 12}
        y={lH / 2 + 25}
        width={50}
        height={10}
        align="center"
        verticalAlign="middle"
        text={`${leftH} m`}
        opacity={0.8}
        rotation={-90}
      />
    </Group>
  );
};

const Rectangle = ({ x, y, onRClick }) => {
  return (
    <Rect
      x={x * 72}
      y={y * 72}
      width={70}
      height={70}
      stroke={"rgb(131,146,171,0.1)"}
      strokeWidth={1}
      onContextMenu={onRClick}
    />
  );
};

const CanvasSite = ({ width = 1680, height = 780, site }) => {
  const topW = site.topLength;
  const leftH = site.leftLength;

  const [stageScale, setStageScale] = useState(1);
  const [stageX, setStageX] = useState(70);
  const [stageY, setStageY] = useState(70);

  const [routerImage, setRouterImage] = useState(new window.Image());

  useEffect(() => {
    const img = new window.Image();
    img.src = routerIcon;
    setRouterImage(img);
  }, []);

  const handleDragStart = (e) => {
    e.target.setAttrs({
      shadowOffset: {
        x: 15,
        y: 15,
      },
      scaleX: 1.1,
      scaleY: 1.1,
    });
  };
  const handleDragEnd = (e) => {
    e.target.to({
      duration: 0.5,
      easing: Konva.Easings.ElasticEaseOut,
      scaleX: 1,
      scaleY: 1,
      shadowOffsetX: 5,
      shadowOffsetY: 5,
    });
  };

  const pulseShape = (shape) => {
    // use Konva methods to animate a shape
    shape.to({
      scaleX: 1.5,
      scaleY: 1.5,
      onFinish: () => {
        shape.to({
          scaleX: 1,
          scaleY: 1,
        });
      },
    });
  };

  const handleGwClick = (e) => {
    // another way to access Konva nodes is to just use event object
    const shape = e.target;
    pulseShape(shape);
    // prevent click on stage
    e.cancelBubble = true;
  };

  const handleRectClick = (e) => {
    e.evt.preventDefault();
    const shape = e.target;
    const attrs = shape.getAttrs();
    console.log("click", attrs);
    shape.setAttrs({ fill: attrs.fill == "#e9ecef" ? "white" : "#e9ecef" });
  };

  function renderSite({ id, topLength, leftLength }) {
    const elements = [];
    elements.push(<Site key={`site_${id}`} x={-2} y={-2} topW={topLength} leftH={leftLength} />);

    /*elements.push(
     
    );
*/
    for (let x = 0; x < topLength; x += 1) {
      for (let y = 0; y < leftLength; y += 1) {
        elements.push(<Rectangle key={`rec-${x}-${y}`} x={x} y={y} onRClick={handleRectClick} />);
      }
    }

    site.gateways.map((gw) => {
      const locationAnchor = [
        [0, 0],
        [topLength / 2, leftLength / 2],
        [topLength / 2, 0],
        [0, 0],
        [topLength, 0],
        [topLength / 2, leftLength],
        [0, leftLength],
        [topLength, leftLength],
        [0, leftLength / 2],
        [topLength, leftLength / 2],
      ];
      elements.push(
        <Group
          key={gw.macAddress}
          id={gw.macAddress}
          x={locationAnchor[gw.location][0] * 70}
          y={locationAnchor[gw.location][1] * 70}
          draggable
          onDragStart={handleDragStart}
          onDragEnd={handleDragEnd}
          onClick={handleGwClick}
        >
          <RegularPolygon
            radius={70}
            sides={6}
            width={100}
            height={100}
            fill="white"
            stroke="#89b717"
            strokeWidth={1}
            opacity={1}
            shadowColor="black"
            shadowBlur={10}
            shadowOpacity={0.6}
          />
          <Image x={-35} y={-25} width={70} height={50} image={routerImage} />

          <Text x={-38} y={65} fill={"black"} text={`${gw.macAddress}`}></Text>
          <Text x={-38} y={82} fill={"#82d616"} text={`${gw.name}`}></Text>
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
    setStageScale(newScale);
    stage.position({
      x: -(mousePointTo.x - stage.getPointerPosition().x / newScale) * newScale,
      y: -(mousePointTo.y - stage.getPointerPosition().y / newScale) * newScale,
    });
    stage.batchDraw();
  }

  return (
    <Stage
      id={"stage"}
      height={height}
      onWheel={handleWheel}
      onDragMove={() => {}}
      onDragEnd={() => {}}
      scaleX={stageScale}
      scaleY={stageScale}
      x={stageX}
      y={stageY}
      draggable={true}
      width={width}
    >
      <Layer>{renderSite(site)}</Layer>
    </Stage>
  );
};

export default CanvasSite;
