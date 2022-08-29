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
import React, { useState } from "react";
import { fetchPermissions } from "utils/query-keys";
import { getPermissions, savePermissions } from "api/admin";
import Permissions from "./permissions";

const RoleConfiguration = ({ item, onSave, onClose }) => {
  const { isSuccess, data } = useQuery([fetchPermissions, item.id], getPermissions);
  const [permissions, setPermissions] = useState([]);

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
      mutation.mutate(permissions);
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
                readOnly: false,
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

            {isSuccess && <Permissions data={data.permissions} onChange={setPermissions} />}

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
