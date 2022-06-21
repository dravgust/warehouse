import { useState } from "react";
import SuiAvatar from "components/SuiAvatar";
import SuiBox from "components/SuiBox";
import Menu from "@mui/material/Menu";

import LanguageItem from "examples/Items/LanguageItem";

import englishIcon from "assets/images/flags/united-kingdom.svg";
import hebrewIcon from "assets/images/flags/israel.svg";

// Soft UI Dashboard React context
import { setDirection, useSoftUIController } from "context";
import Tooltip from "@mui/material/Tooltip";

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
      <Tooltip title="Language">
        <SuiAvatar
          src={direction === "ltr" ? englishIcon : hebrewIcon}
          alt={direction === "ltr" ? "English" : "Hebrew"}
          size="sm"
          variant="rounded"
          sx={{
            boxShadow:
              "0rem 0.25rem 0.375rem -0.0625rem rgb(20 20 20 / 12%), 0rem 0.125rem 0.25rem -0.0625rem rgb(20 20 20 / 7%)",
              cursor: "pointer"
          }}
          onClick={handleOpenMenu}
        />
      </Tooltip>
      {renderLangMenu({ horizontal: direction === "ltr" ? "left" : "right" })}
    </SuiBox>
  );
}
