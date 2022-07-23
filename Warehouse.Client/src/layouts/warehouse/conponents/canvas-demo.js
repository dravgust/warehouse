import React, { Component } from "react";
import { Stage, Layer, RegularPolygon } from "react-konva";

const Hexagon = ({ x, y }) => {
  return (
    <RegularPolygon
      x={x * 121 + 60 * (y % 2)}
      y={y * 105}
      sides={6}
      radius={70}
      stroke="black"
      strokeWidth={1}
    />
  );
};

class CanvasDemo extends Component {
  state = {
    stageScale: 1,
    stageX: 0,
    stageY: 0,
  };

  hexagons() {
    const hexagons = [];
    for (let x = 0; x < 50; x += 1) {
      for (let y = 0; y < 50; y += 1) {
        hexagons.push(<Hexagon key={`hex-${x}-${y}`} x={x} y={y} />);
      }
    }
    return hexagons;
  }
  handleWheel = (e) => {
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
  };

  render() {
    return (
      <Stage
        height={780}
        onWheel={this.handleWheel}
        scaleX={this.state.stageScale}
        scaleY={this.state.stageScale}
        x={this.state.stageX}
        y={this.state.stageY}
        draggable={true}
        width={1680}
      >
        <Layer>{this.hexagons()}</Layer>
      </Stage>
    );
  }
}
export default CanvasDemo;
