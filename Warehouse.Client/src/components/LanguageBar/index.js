import { useState } from "react";
import SuiBox from "components/SuiBox";
import Menu from "@mui/material/Menu";
import LanguageItem from "examples/Items/LanguageItem";
import englishIcon from "assets/images/flags/united-kingdom.svg";
import hebrewIcon from "assets/images/flags/israel.svg";
import { setDirection, useSoftUIController } from "context";
import Icon from "@mui/material/Icon";
import SidenavCollapse from "../../examples/Sidenav/SidenavCollapse";

export default function LanguageBar() {
  const [controller, dispatch] = useSoftUIController();
  const { direction } = controller;
  const [openMenu, setOpenMenu] = useState(false);
  const handleOpenMenu = (event) => setOpenMenu(event.currentTarget);
  const handleCloseMenu = () => setOpenMenu(false);

  const renderLangMenu = ({ horizontal }) => (
    <Menu
      anchorEl={openMenu}
      id="language-menu"
      anchorReference={null}
      anchorOrigin={{
        vertical: "bottom",
        horizontal,
      }}
      open={Boolean(openMenu)}
      onClose={handleCloseMenu}
      onClick={handleCloseMenu}
      sx={{ mt: 2 }}
    >
      <LanguageItem
        image={<img src={englishIcon} alt="English" />}
        title={["English"]}
        onClick={() => setDirection(dispatch, "ltr")}
      />
      <LanguageItem
        image={<img src={hebrewIcon} alt="Hebrew" />}
        title={["Hebrew"]}
        onClick={() => setDirection(dispatch, "rtl")}
      />
    </Menu>
  );

  return (
    <SuiBox>
      <SidenavCollapse
        //name={direction === "ltr" ? "English" : "Hebrew"}
        name={"Language"}
        onClick={handleOpenMenu}
        icon={<Icon fontSize="12px">language</Icon>}
      />
      {renderLangMenu({ horizontal: direction === "ltr" ? "left" : "right" })}
    </SuiBox>
  );
}
