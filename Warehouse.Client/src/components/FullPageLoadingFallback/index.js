import Backdrop from "@mui/material/Backdrop";
import { ScaleLoader } from "react-spinners";

const FullPageErrorFallback = () => {
  return (
    <Backdrop sx={{ color: "#fff", zIndex: (theme) => theme.zIndex.drawer + 1 }} open={true}>
      <ScaleLoader
        loading={true}
        color={"#17c1e8"}
        height={"100px"}
        cssOverride={{ position: "absolute", display: "inherit", top: "50%", left: "50%" }}
      />
    </Backdrop>
  );
};
export default FullPageErrorFallback;
