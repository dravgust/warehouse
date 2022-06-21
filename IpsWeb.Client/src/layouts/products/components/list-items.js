import React, { useState } from "react";
import { useQuery } from "react-query";
import {client} from 'utils/api-client'
import * as auth from 'auth-provider'

function ListItems() {

  const [page, setPage] = useState(0);

  const fetchItems = async (page) => {
    const token = await auth.getToken();
    const res = await client(`items?page=${page}&size=10`, {token});
    return res;
  };
  const { isLoading, error, data, isSuccess } = useQuery(["list-items", page], () => fetchItems(page), { keepPreviousData:false });

  return (
    <div>
      <button onClick={() => setPage((old) => Math.max(0, old - 1))}>
        {" "}
        -{" "}
      </button>
      <button onClick={() => setPage((old) => old + 1)}> + </button>

      <p> {page} </p>
      {isSuccess &&
        data.data.map((item) => (
          <div key={item._id}>
            <p>{item.name}</p>
            <p>{item._id}</p>
          </div>
        ))}
      {isLoading && <p>Loading..</p>}
      {error && <p>Error occurred!</p>}
    </div>
  );
}

export default ListItems;