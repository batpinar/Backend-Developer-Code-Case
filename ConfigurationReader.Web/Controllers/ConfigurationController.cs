using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConfigurationReader.Core.Entities;
using ConfigurationReader.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConfigurationReader.Web.Controllers;

public class ConfigurationController : Controller
{
    private readonly IConfigurationService _configurationService;
    private readonly string _applicationName;
    private readonly ILogger<ConfigurationController> _logger;
    private readonly ConfigurationReader.Core.ConfigurationReader _configurationReader;

    public ConfigurationController(
        IConfigurationService configurationService,
        ConfigurationReader.Core.ConfigurationReader configurationReader,
        IConfiguration configuration,
        ILogger<ConfigurationController> logger)
    {
        _configurationService = configurationService;
        _configurationReader = configurationReader;
        _applicationName = configuration["ApplicationName"] ?? "ConfigurationReader.Web";
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var configurations = await _configurationService.GetAllAsync(_applicationName);
            return View(configurations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting configurations");
            return View(new List<Configuration>());
        }
    }

    public IActionResult Create()
    {
        return View(new Configuration { ApplicationName = _applicationName });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Type,Value,ApplicationName")] Configuration configuration)
    {
        if (ModelState.IsValid)
        {
            try
            {
                configuration.CreatedDate = DateTime.UtcNow;
                configuration.IsActive = true;
                await _configurationService.AddAsync(configuration);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating configuration");
                ModelState.AddModelError("", "An error occurred while creating the configuration.");
            }
        }
        return View(configuration);
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var configuration = await _configurationService.GetByIdAsync(id);
            if (configuration == null)
            {
                return NotFound();
            }
            return View(configuration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting configuration for edit");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Type,Value,ApplicationName,IsActive")] Configuration configuration)
    {
        if (id != configuration.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                configuration.UpdatedDate = DateTime.UtcNow;
                await _configurationService.UpdateAsync(configuration);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating configuration");
                ModelState.AddModelError("", "An error occurred while updating the configuration.");
            }
        }
        return View(configuration);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _configurationService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting configuration");
            return RedirectToAction(nameof(Index));
        }
    }
} 