import { useEffect, useState } from "react";
import { useFormik } from "formik";
import { useMutation } from "react-query";
import * as yup from "yup";
// Soft UI Dashboard React components
import Stack from "@mui/material/Stack";
import { Icon, TextField, Box, FormControlLabel } from "@mui/material";
import SuiAlert from "components/SuiAlert";
import SuiButton from "components/SuiButton";
import DeletePromt from "./delete-promt";
import { deleteAlert, saveAlert } from "services/warehouse-service";
import Checkbox from "@mui/material/Checkbox";
import { pink } from "@mui/material/colors";
import SuiBox from "../../../../components/SuiBox";

const validationSchema = yup.object({
  name: yup.string("Enter alert name"),
});

export default function AlertForm({ onSave = () => {}, onDelete = () => {}, item = {} }) {
  const mutation = useMutation((item) => saveAlert(item), {
    onSuccess: () => {
      formik.resetForm();
      return onSave();
    },
  });

  const handleDelete = async (item) => {
    try {
      await deleteAlert(item);
      return onDelete();
    } catch (err) {
      console.log("delete-item", err);
    }
  };

  const formik = useFormik({
    enableReinitialize: true,
    initialValues: {
      name: item.name ? item.name : "",
      checkPeriod: item ? item.checkPeriod : 0,
      enabled: Boolean(item && item.enabled),
    },
    validationSchema: validationSchema,
    onSubmit: (values) => {
      mutation.mutate(values);
    },
  });

  return (
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
          {mutation.error.title || mutation.error.error || "Some error occurred!"}
        </SuiAlert>
      )}

      <TextField
        fullWidth
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
        id="checkPeriod"
        name="checkPeriod"
        label="Check Period"
        value={formik.values.checkPeriod}
        onChange={formik.handleChange}
        error={formik.touched.checkPeriod && Boolean(formik.errors.checkPeriod)}
        helperText={formik.touched.checkPeriod && formik.errors.checkPeriod}
      />

      <SuiBox px={3} pt={2}>
        <FormControlLabel
          control={
            <Checkbox
              checked={formik.values.enabled}
              onChange={formik.handleChange}
              id="enabled"
              name="enabled"
            />
          }
          label="Enabled"
        />
      </SuiBox>

      <Stack my={2} py={2} direction="row" spacing={1} justifyContent="end">
        <DeletePromt
          renderButton={(handleClickOpen) => (
            <SuiButton variant="text" color="error" onClick={handleClickOpen} disabled={!item.id}>
              <Icon>delete</Icon>&nbsp;delete
            </SuiButton>
          )}
          onDelete={() => handleDelete(item)}
        />
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
  );
}
