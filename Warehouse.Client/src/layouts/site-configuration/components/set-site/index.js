import { Card, Icon, Tooltip, IconButton, Box, TextField, Stack } from "@mui/material";
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import { useFormik } from "formik";
import SuiAlert from "components/SuiAlert";
import SuiButton from "components/SuiButton";
import { useMutation } from "react-query";
import * as yup from "yup";
import PropTypes from "prop-types";
import { setSite } from "api/warehouse";

export default function SetSite({ item, onClose, onSave = () => {} }) {
  const mutation = useMutation(setSite, {
    onSuccess: () => {
      formik.resetForm();
      return onSave();
    },
  });

  const validationSchema = yup.object({
    name: yup
      .string("Enter product name")
      .min(3, "Name should be of minimum 3 characters length")
      .required("Name is required"),
  });

  const formik = useFormik({
    enableReinitialize: true,
    initialValues: {
      id: item ? item.id : "",
      name: item ? item.name : "",
      topLength: item ? item.topLength : 0,
      leftLength: item ? item.leftLength : 0,
      error: item ? item.error : 0,
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
          <SuiTypography variant="h6">Set Site</SuiTypography>
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

            <Stack direction="row" spacing={2} alignItems="center">
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
            </Stack>

            <Stack direction="row" spacing={2} alignItems="center">
              <TextField
                fullWidth
                sx={{
                  "& .MuiInputBase-input": { width: "100% !important" },
                }}
                id="topLength"
                name="topLength"
                label="topLength"
                type={"number"}
                value={formik.values.topLength}
                onChange={formik.handleChange}
                error={formik.touched.topLength && Boolean(formik.errors.topLength)}
                helperText={formik.touched.topLength && formik.errors.topLength}
              />
              <TextField
                fullWidth
                sx={{
                  "& .MuiInputBase-input": { width: "100% !important" },
                }}
                id="leftLength"
                name="leftLength"
                label="leftLength"
                type={"number"}
                value={formik.values.leftLength}
                onChange={formik.handleChange}
                error={formik.touched.leftLength && Boolean(formik.errors.leftLength)}
                helperText={formik.touched.leftLength && formik.errors.leftLength}
              />
            </Stack>
            <Stack direction="row" spacing={2} alignItems="center">
              <TextField
                fullWidth
                sx={{
                  "& .MuiInputBase-input": { width: "100% !important" },
                }}
                id="error"
                name="error"
                label="error"
                type={"number"}
                value={formik.values.error}
                onChange={formik.handleChange}
                error={formik.touched.error && Boolean(formik.errors.error)}
                helperText={formik.touched.error && formik.errors.error}
              />
            </Stack>
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
}

// Setting default values
SetSite.defaultProps = {};

// Typechecking props
SetSite.propTypes = {
  item: PropTypes.object.isRequired,
  onClose: PropTypes.func.isRequired,
};
