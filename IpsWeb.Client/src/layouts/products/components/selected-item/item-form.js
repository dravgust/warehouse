import * as React from "react";
import { useFormik } from "formik";
import { useMutation } from "react-query";
import * as yup from "yup";
// Soft UI Dashboard React components
import Stack from "@mui/material/Stack";
import {
  Icon,
  TextField,
  Box,
} from "@mui/material";
import * as auth from "auth-provider";
import { client } from "utils/api-client";
import Autocomplete from "@mui/material/Autocomplete";
import SuiAlert from "components/SuiAlert";
import SuiButton from "components/SuiButton";
import DeletePromt from "./delete-promt";

const validationSchema = yup.object({
  name: yup
    .string("Enter product name")
    .min(3, "Name should be of minimum 3 characters length")
    .required("Name is required"),
  description: yup
    .string("Enter product description")
    .min(3, "Description should be of minimum 3 characters length"),
  macAddress: yup
    .string("Enter MAC address")
    .min(5, "MAC address should be of minimum 6 characters length"),
  metadata: yup.array().of(
    yup.object({
      value: yup.string().when(["isRequired"], {
        is: (isRequired) => isRequired,
        then: yup.string().required("Value is required"),
        otherwise: yup.string(),
      }),
    })
  ),
});

export default function ItemForm({ onSave = () => {}, onDelete = () => {}, item = {}, beaconsRegistered = [] }) {

  const saveItem = async (item) => {
    const token = await auth.getToken();
    const res = await client(`items/set`, {
      data: item,
      token,
    });
    return res;
  };

  const mutation = useMutation((item) => saveItem(item), {
    onSuccess: () => {
      formik.resetForm();
      return onSave();
    },
  });

  const handleDelete = async (item) => {
    const token = await auth.getToken();
    try {
      await client(`items/${item.id}/delete`, { token });
      return onDelete();
    } catch (err) {
      console.log("delete-item", err);
    }
  };

  const formik = useFormik({
    enableReinitialize: true,
    initialValues: {
      id: item ? item.id : "",
      name: item ? item.name : "",
      description: item ? item.description : "",
      macAddress: item ? item.macAddress : "",
      metadata: item && item.metadata ? item.metadata : [],
    },
    validationSchema: validationSchema,
    onSubmit: (values) => {
      mutation.mutate(values);
    },
  });

  const handleBlur = (e) => {
    console.log("Blur:", e.target.value);
  };

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
        <SuiAlert style={{fontSize:"12px"}} color={"error"} dismissible>
          {mutation.error.title || mutation.error.error}
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

      <Stack direction="row" spacing={2} alignItems="center">
        <Autocomplete
        disablePortal
          onBlur={handleBlur}
          options={["", ...beaconsRegistered]}
          isOptionEqualToValue={(option, value) => option === value}
          sx={{ width: 300 }}
          getOptionLabel={(option) => option}
          onChange={(e, value) => {
            formik.setFieldValue("macAddress", value);
          }}
          value={formik.values.macAddress}
          renderInput={(params) => (
            <TextField
              id="macAddress"
              name="macAddress"
              label="MacAddress"
              {...params}
              error={formik.touched.macAddress && Boolean(formik.errors.macAddress)}
              helperText={formik.touched.macAddress && formik.errors.macAddress}
            />
          )}
        />
        {item.macAddress ? <Icon>link</Icon> : <Icon>link_off</Icon>}
        
      </Stack>
      <TextField
        fullWidth
        id="description"
        name="description"
        label="Description"
        type="text"
        multiline
        rows={4}
        value={formik.values.description}
        onChange={formik.handleChange}
        error={formik.touched.description && Boolean(formik.errors.description)}
        helperText={formik.touched.description && formik.errors.description}
      />

      {formik.values.metadata &&
        formik.values.metadata.map(({ key, value, type }, index) => (
          <TextField
          key={index}
            fullWidth
            label={key}
            id={`metadata[${index}].value`}
            name={`metadata[${index}].value`}
            value={value}
            type={type || "text"}
            InputLabelProps={{ shrink: true }}
            onChange={formik.handleChange}
            error={
              formik.touched.metadata &&
              formik.touched.metadata[index].value &&
              formik.errors.metadata &&
              formik.errors.metadata[index] &&
              Boolean(formik.errors.metadata[index].value)
            }
            helperText={
              formik.touched.metadata &&
              formik.touched.metadata[index].value &&
              formik.errors.metadata &&
              formik.errors.metadata[index] &&
              formik.errors.metadata[index].value
            }
          />
        ))}

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
