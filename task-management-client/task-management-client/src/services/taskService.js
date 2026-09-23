import api from "../api/axios";

export const getDashboard = async () => {
  const response = await api.get(
    "/Tasks/dashboard"
  );

  return response.data;
};

export const getTasks = async (
  filters = {}
) => {
  const response = await api.get(
    "/Tasks",
    {
      params: filters,
    }
  );

  return response.data;
};

export const getTaskById = async (id) => {
  const response = await api.get(
    `/Tasks/${id}`
  );

  return response.data;
};

export const createTask = async (task) => {
  const response = await api.post(
    "/Tasks",
    task
  );

  return response.data;
};

export const updateTask = async (
  id,
  task
) => {
  const response = await api.put(
    `/Tasks/${id}`,
    task
  );

  return response.data;
};

export const updateTaskStatus = async (
  id,
  status
) => {
  const response = await api.patch(
    `/Tasks/${id}/status`,
    {
      status,
    }
  );

  return response.data;
};

export const deleteTask = async (id) => {
  const response = await api.delete(
    `/Tasks/${id}`
  );

  return response.data;
};

// Admin only - Get task activity history
export const getTaskActivity = async (
  taskId
) => {
  const response = await api.get(
    `/Tasks/${taskId}/activity`
  );

  return response.data;
};