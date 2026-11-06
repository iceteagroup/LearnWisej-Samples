using System;
using System.IO;
using FieldOps.Services;
using Wisej.Hybrid;
using Wisej.Web;

namespace FieldOps.Offline;

/// <summary>
/// The Local app's session entry point (Offline.json "startup"). Same screens as the server;
/// different data: the orders the Remote session saved on this device, and every change made here
/// recorded for the next Remote sign-in.
/// </summary>
static class Program
{
	static void Main()
	{
		AppMode.IsLocal = true;

		// No console in a desktop app: the Local app's log goes to a file on the device.
		OpsLog.FilePath = Path.Combine(Path.GetTempPath(), "FieldOps.Offline.log");
		OpsLog.Info($"local app started at {Application.Url} (Device.Valid={Device.Valid}, OfflineUrl={Device.System.OfflineUrl})");

		DeviceSync.Snapshot snapshot = null;
		try
		{
			snapshot = DeviceSync.LoadSnapshot();
		}
		catch (Exception ex)
		{
			OpsLog.Error("reading the snapshot on the device", ex);
		}

		if (snapshot != null)
		{
			WorkOrderStore.Shared = new WorkOrderStore(snapshot.Orders);
			OpsLog.Info($"local app: {snapshot.Orders.Count} orders synced at {snapshot.SyncedAt:HH:mm} for {snapshot.TechnicianId}");
		}
		else
		{
			// Never synced on this device: an empty list, not the server's seed data.
			WorkOrderStore.Shared = new WorkOrderStore(new System.Collections.Generic.List<Model.WorkOrder>());
			OpsLog.Info("local app: no snapshot on this device yet");
		}

		var service = WorkOrderService.Current;
		var technician = service.FindTechnician(snapshot?.TechnicianId ?? "dana");
		SignIn.Complete(technician);

		Session.ListPage = new WorkOrdersPage();
		Session.ListPage.Show();
	}
}
