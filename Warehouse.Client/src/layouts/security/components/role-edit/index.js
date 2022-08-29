import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import { Box, Card, Icon, IconButton, Stack, TextField, Tooltip } from "@mui/material";
import SuiAlert from "components/SuiAlert";
import SuiButton from "components/SuiButton";
import { useMutation, useQuery } from "react-query";
import { queryClient } from "context/app.context";
import * as yup from "yup";
import { useFormik } from "formik";
import PropTypes from "prop-types";
import React, { useEffect, useState } from "react";
import Checkbox from "@mui/material/Checkbox";
import Table from "examples/Tables/Table";
import { fetchPermissions } from "utils/query-keys";
import { getPermissions, savePermissions } from "api/admin";
import { SecurityPermissions } from "services/security-provider";

const RoleConfiguration = ({ item, onSave, onClose }) => {
  const { isSuccess, data } = useQuery([fetchPermissions, item.id], getPermissions);
  const [permissions, setPermissions] = useState([]);
  useEffect(() => {
    if (isSuccess) {
      const result = data.permissions.map((item) => ({
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
  }, [item, isSuccess]);

  const handleChange = (event) => {
    const perm = event.currentTarget.getAttribute("data-per");
    const result = permissions.map((item) =>
      item.id === event.target.name ? { ...item, [perm]: event.target.checked } : item
    );
    setPermissions(result);
  };

  const mutation = useMutation(savePermissions, {
    onSuccess: () => {
      formik.resetForm();
      queryClient.resetQueries(fetchPermissions);
      return onSave();
    },
  });

  const validationSchema = yup.object({
    name: yup
      .string("Enter role name")
      .min(3, "Name should be of minimum 3 characters length")
      .required("Name is required"),
  });

  const formik = useFormik({
    enableReinitialize: true,
    initialValues: {
      id: item ? item.id : "",
      name: item ? item.name : "",
      description: item ? item.description : "",
    },
    validationSchema: validationSchema,
    onSubmit: (values) => {
      const result = permissions.map((item) => {
        const perm =
          (item[SecurityPermissions.View] && SecurityPermissions.View) |
          (item[SecurityPermissions.Add] && SecurityPermissions.Add) |
          (item[SecurityPermissions.Edit] && SecurityPermissions.Edit) |
          (item[SecurityPermissions.Delete] && SecurityPermissions.Delete) |
          (item[SecurityPermissions.Execute] && SecurityPermissions.Execute) |
          (item[SecurityPermissions.Grant] && SecurityPermissions.Grant);
        return { ...item, permissions: perm };
      });
      mutation.mutate(result);
    },
  });

  return (
    <Card>
      <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
        <SuiBox>
          <SuiTypography variant="h6">Role Configuration</SuiTypography>
        </SuiBox>
        <SuiBox display="flex" alignItems="center" mt={{ xs: -1, sm: 0 }}>
          <IconButton size="xl" color="inherit" onClick={onClose}>
            <Tooltip title="Close">
              <Icon>close</Icon>
            </Tooltip>
          </IconButton>
        </SuiBox>
      </SuiBox>
      <SuiBox
        component="div"
        display="flex"
        justifyContent="space-between"
        alignItems="flex-start"
        bgColor="rgba(203, 12, 159, 0.08)"
        borderRadius="lg"
        mb={2}
        mx={2}
        px={2}
        pt={2}
        style={{ border: "1px solid rgba(0, 0, 0, 0.125)" }}
      >
        <SuiBox width="100%" display="flex" flexDirection="column">
          <Box
            component="form"
            onSubmit={formik.handleSubmit}
            sx={{
              "& .MuiTextField-root": { m: 1 },
            }}
            noValidate
            autoComplete="off"
          >
            {mutation.isError && (
              <SuiAlert style={{ fontSize: "12px" }} color={"error"} dismissible>
                {mutation.error.title || mutation.error.error}
              </SuiAlert>
            )}

            <TextField
              fullWidth
              inputProps={{
                readOnly: true,
              }}
              sx={{
                "& .MuiInputBase-input": { width: "100% !important" },
              }}
              id="name"
              name="name"
              label="Name"
              value={formik.values.name}
              onChange={formik.handleChange}
              error={formik.touched.name && Boolean(formik.errors.name)}
              helperText={formik.touched.name && formik.errors.name}
            />

            <TextField
              fullWidth
              inputProps={{
                readOnly: true,
              }}
              sx={{
                "& .MuiOutlinedInput-input": { width: "100%!important" },
              }}
              id="description"
              name="description"
              label="Description"
              type="text"
              multiline
              maxRows={10}
              value={formik.values.description || ""}
              onChange={formik.handleChange}
              error={formik.touched.description && Boolean(formik.errors.description)}
              helperText={formik.touched.description && formik.errors.description}
            />

            <SuiBox
              px={1}
              py={2}
              sx={{
                "& .MuiTableContainer-root": {
                  paddingTop: "20px",
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

            <Stack my={2} px={1} direction="row" spacing={1} justifyContent="end">
              <SuiButton color="success" variant="contained" type="submit">
                {mutation.isLoading ? (
                  "Loading..."
                ) : (
                  <>
                    <Icon>save</Icon>&nbsp;save
                  </>
                )}
              </SuiButton>
            </Stack>
          </Box>
        </SuiBox>
      </SuiBox>
    </Card>
  );
};

// Setting default values
RoleConfiguration.defaultProps = {};

// Typechecking props
RoleConfiguration.propTypes = {
  item: PropTypes.object.isRequired,
  onClose: PropTypes.func.isRequired,
};

export default RoleConfiguration;
