import { ScaleLoader } from "react-spinners";

const Loader = () => {
  return (
    <ScaleLoader
      loading={true}
      color={"#17c1e8"}
      height={"100px"}
      cssOverride={{ position: "absolute", display: "inherit", top: "50%", left: "50%" }}
    />
  );
};

export default Loader;
