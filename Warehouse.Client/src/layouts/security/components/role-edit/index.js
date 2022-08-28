import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import { Box, Card, Icon, IconButton, Stack, TextField, Tooltip } from "@mui/material";
import SuiAlert from "components/SuiAlert";
import SuiButton from "components/SuiButton";
import { useMutation, useQuery } from "react-query";
import * as yup from "yup";
import { useFormik } from "formik";
import PropTypes from "prop-types";
import React from "react";
import Checkbox from "@mui/material/Checkbox";
import Table from "examples/Tables/Table";
import { fetchObjects } from "utils/query-keys";
import { getObjects } from "api/admin";

const RoleConfiguration = ({ item, onSave, onClose }) => {
  const { isSuccess, data: objects, isLoading, error } = useQuery([fetchObjects], getObjects);

  console.log("role-edit", objects);
  const res = objects.items.map((obj, index) => {
    return { id: obj.id };
  });

  const [state, setState] = React.useState({
    gilad: true,
    jason: false,
    antoine: false,
  });

  const handleChange = (event) => {
    setState({
      ...state,
      [event.target.name]: event.target.checked,
    });
  };

  const { gilad, jason, antoine } = state;
  const checkedError = [gilad, jason, antoine].filter((v) => v).length !== 2;

  const mutation = useMutation(() => {}, {
    onSuccess: () => {
      formik.resetForm();
      return onSave();
    },
  });

  const validationSchema = yup.object({
    name: yup
      .string("Enter role name")
      .min(3, "Name should be of minimum 3 characters length")
      .required("Name is required"),
    description: yup
      .string("Enter role description")
      .min(3, "Description should be of minimum 3 characters length"),
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
      mutation.mutate(values);
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
                rows={
                  isSuccess
                    ? objects.items.map((item, index) => ({
                        "security objects": (
                          <SuiBox display="flex" flexDirection="column" px={1}>
                            <SuiTypography variant="button" fontWeight="medium">
                              {item.name ? item.name : "n/a"}
                            </SuiTypography>
                          </SuiBox>
                        ),
                        view: <Checkbox name={`${item.id}_${index}`} />,
                        add: <Checkbox />,
                        edit: <Checkbox />,
                        delete: <Checkbox />,
                        execute: <Checkbox />,
                        grant: <Checkbox />,
                        "": "",
                      }))
                    : []
                }
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
