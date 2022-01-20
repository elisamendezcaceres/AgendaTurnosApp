﻿using AgendaTurnosApp.Repositories.Shifts;
using AgendaTurnosApp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgendaTurnosApp.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private readonly IShiftRepository _shiftRepository;

        public ShiftController(IShiftRepository shiftRepository)
        {
            _shiftRepository = shiftRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<Shift>> Get()
        {
            return await _shiftRepository.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<Shift> Get(int id)
        {
            return await _shiftRepository.GetDetails(id);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Shift Shift)
        {
            if (Shift == null)
                return BadRequest();
            if (Shift.PatientId == 0)
                ModelState.AddModelError("PatientId", "Debe seleccionar el Paciente");
            if (Shift.DoctorId == 0)
                ModelState.AddModelError("DoctorId", "Debe seleccionar el Doctor");

            var shiftList = await _shiftRepository.GetAll();
            if(shiftList.Any( 
                s=> s.DoctorId == Shift.DoctorId && 
                    s.PatientId == Shift.PatientId && 
                    s.ShiftDate.ToShortDateString() == Shift.ShiftDate.ToShortDateString()
                    ))
            {                
                ModelState.AddModelError(nameof(Shift.ShiftDate), "El paciente ya tiene un turno asignado en la fecha seleccionada.");                
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _shiftRepository.InsertShift(Shift);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Shift Shift)
        {
            if (Shift == null)
                return BadRequest();
            if (Shift.PatientId == 0)
                ModelState.AddModelError("PatientId", "Debe seleccionar el Paciente");
            if (Shift.DoctorId == 0)
                ModelState.AddModelError("DoctorId", "Debe seleccionar el Doctor");

            IEnumerable<Shift> shiftList = await _shiftRepository.GetAll();
            if (shiftList.Any(
                s => s.DoctorId == Shift.DoctorId &&
                    s.PatientId == Shift.PatientId &&
                    s.ShiftDate.ToShortDateString() == Shift.ShiftDate.ToShortDateString()
                    ))
            {
                ModelState.AddModelError(nameof(Shift.ShiftDate), "El paciente ya tiene un turno asignado en la fecha seleccionada.");
                
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _shiftRepository.UpdateShift(Shift);
                return NoContent();
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _shiftRepository.DelteShift(id);
        }

        [HttpGet("date")]
        public async Task<IEnumerable<Shift>> GetShiftByDate()
        {
            DateTime date = DateTime.Today;
            var list = await _shiftRepository.GetAllByDate(date);
            return list;           
        }
    }
}