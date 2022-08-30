import React, { useEffect, useState } from "react";
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import PropTypes from "prop-types";
import Checkbox from "@mui/material/Checkbox";
import Table from "examples/Tables/Table";
import { SecurityPermissions } from "services/security-provider";

const SetPermissions = ({ data, onChange }) => {
  const [permissions, setPermissions] = useState([]);

  useEffect(() => {
    if (data) {
      const result = data.map((item) => ({
        ...item,
        [SecurityPermissions.View]:
          (item.permissions & SecurityPermissions.View) === SecurityPermissions.View,
        [SecurityPermissions.Add]:
          (item.permissions & SecurityPermissions.Add) === SecurityPermissions.Add,
        [SecurityPermissions.Edit]:
          (item.permissions & SecurityPermissions.Edit) === SecurityPermissions.Edit,
        [SecurityPermissions.Delete]:
          (item.permissions & SecurityPermissions.Delete) === SecurityPermissions.Delete,
        [SecurityPermissions.Execute]:
          (item.permissions & SecurityPermissions.Execute) === SecurityPermissions.Execute,
        [SecurityPermissions.Grant]:
          (item.permissions & SecurityPermissions.Grant) === SecurityPermissions.Grant,
      }));
      setPermissions(result);
    }
  }, [data]);

  const handleChange = (event) => {
    const perm = event.currentTarget.getAttribute("data-per");
    const result = permissions.map((item) =>
      item.id === event.target.name ? { ...item, [perm]: event.target.checked } : item
    );
    setPermissions(result);

    const changed = result.map((item) => {
      const permissions =
        (item[SecurityPermissions.View] && SecurityPermissions.View) |
        (item[SecurityPermissions.Add] && SecurityPermissions.Add) |
        (item[SecurityPermissions.Edit] && SecurityPermissions.Edit) |
        (item[SecurityPermissions.Delete] && SecurityPermissions.Delete) |
        (item[SecurityPermissions.Execute] && SecurityPermissions.Execute) |
        (item[SecurityPermissions.Grant] && SecurityPermissions.Grant);
      return { ...item, permissions };
    });
    return onChange(changed);
  };

  return (
    <SuiBox
      px={1}
      py={2}
      sx={{
        "& .MuiTableContainer-root": {
          paddingTop: "20px",
          boxShadow: 0,
          border: "0.0625rem solid #d2d6da",
        },
        "& .MuiTableRow-root": {
          "& th:first-of-type, th:last-child": {
            borderBottom: 0,
            visibility: "hidden",
          },
          "& td:not(:first-of-type):not(:last-child)": {
            borderBottom: ({ borders: { borderWidth, borderColor } }) =>
              `${borderWidth[1]} solid ${borderColor}`,
          },
          "& td:not(:last-child)": {
            borderRight: ({ borders: { borderWidth, borderColor } }) =>
              `${borderWidth[1]} solid ${borderColor}`,
          },
        },
      }}
    >
      <Table
        columns={[
          { name: "security objects", align: "right" },
          { name: "view", align: "center" },
          { name: "add", align: "center" },
          { name: "edit", align: "center" },
          { name: "delete", align: "center" },
          { name: "execute", align: "center" },
          { name: "grant", align: "center" },
          { name: "", align: "center" },
        ]}
        rows={permissions.map((item) => ({
          "security objects": (
            <SuiBox display="flex" flexDirection="column" px={1}>
              <SuiTypography variant="button" fontWeight="medium">
                {item.objectName ? item.objectName : "n/a"}
              </SuiTypography>
            </SuiBox>
          ),
          view: (
            <Checkbox
              name={`${item.id}`}
              inputProps={{
                "data-per": SecurityPermissions.View,
              }}
              checked={item[SecurityPermissions.View]}
              onChange={handleChange}
            />
          ),
          add: (
            <Checkbox
              name={`${item.id}`}
              inputProps={{
                "data-per": SecurityPermissions.Add,
              }}
              checked={item[SecurityPermissions.Add]}
              onChange={handleChange}
            />
          ),
          edit: (
            <Checkbox
              name={`${item.id}`}
              inputProps={{
                "data-per": SecurityPermissions.Edit,
              }}
              checked={item[SecurityPermissions.Edit]}
              onChange={handleChange}
            />
          ),
          delete: (
            <Checkbox
              name={`${item.id}`}
              inputProps={{
                "data-per": SecurityPermissions.Delete,
              }}
              checked={item[SecurityPermissions.Delete]}
              onChange={handleChange}
            />
          ),
          execute: (
            <Checkbox
              name={`${item.id}`}
              inputProps={{
                "data-per": SecurityPermissions.Execute,
              }}
              checked={item[SecurityPermissions.Execute]}
              onChange={handleChange}
            />
          ),
          grant: (
            <Checkbox
              name={`${item.id}`}
              inputProps={{
                "data-per": SecurityPermissions.Grant,
              }}
              checked={item[SecurityPermissions.Grant]}
              onChange={handleChange}
            />
          ),
          "": "",
        }))}
      />
    </SuiBox>
  );
};

// Setting default values
SetPermissions.defaultProps = {
  onChange: (permissions) => {},
};

// Typechecking props
SetPermissions.propTypes = {
  data: PropTypes.array.isRequired,
  onChange: PropTypes.func,
};

export default React.memo(SetPermissions);
